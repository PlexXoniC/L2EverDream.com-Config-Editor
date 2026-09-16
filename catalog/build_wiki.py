"""Generates the wiki's settings reference from catalog.json.

One page per category (docs/wiki/Settings-<Category>.md) listing every setting with what it does, what it is saved to,
its default, allowed values and how it relates to other settings, plus an index page
(Settings-Reference.md). Hand-written wiki pages are never touched.

    python catalog/build_wiki.py
"""
import json
import os
import re

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.dirname(HERE)
CATALOG = os.path.join(HERE, "catalog.json")
WIKI = os.path.join(ROOT, "docs", "wiki")
INDEX = "Settings-Reference"

TARGET_WORDS = {
    "server-game": "game server",
    "server-login": "login server",
    "client-option": "game client",
    "client-l2ini": "game client",
    "world-profile": "launcher world settings",
}

EDITOR_WORDS = {
    "toggle": "on/off",
    "number": "number",
    "slider": "slider",
    "choice": "choice",
    "text": "text",
    "list": "list",
    "secret": "password",
    "skill-durations": "skill list",
}

FORMAT_WORDS = {
    "int-list": "numbers separated by commas, e.g. `57,4037`",
    "int-list;": "numbers separated by semicolons, e.g. `100;30;0`",
    "pair-list": "`id,value` pairs separated by semicolons, e.g. `57,2;4037,1.5`",
    "boss-drop-list": "`itemId,min,max,chance` groups separated by semicolons",
    "range-list": "`from,to` level ranges separated by semicolons, e.g. `0,9;10,14`",
    "percent-split-4": "four percentages that add up to 100",
    "time-list": "times as `HH:MM` separated by commas",
    "weekday-list": "day numbers 1–7 separated by commas (1 = Sunday)",
    "hex-color": "a 6-digit colour code, e.g. `00FF00`",
    "coordinates": "`x,y,z` coordinates",
    "buffer-list": "`size x count` pairs separated by semicolons, e.g. `100x8;128x8`",
    "word-list": "words separated by commas",
    "word-list;": "words separated by semicolons",
    "ip-list": "IP addresses separated by commas",
    "host": "an IP address or host name",
    "regex": "a regular expression",
    "skill-duration-list": "`skillId,seconds` pairs (edited on its own page)",
}

PAGE_NAMES = {}  # category id -> wiki page name


def slug(text):
    return re.sub(r"-+", "-", re.sub(r"[^A-Za-z0-9]+", "-", text)).strip("-")


def anchor(heading):
    """GitHub's anchor for a heading."""
    text = heading.lower().replace("&", "").replace("›", "")
    return re.sub(r"-+", "-", re.sub(r"[^a-z0-9]+", "-", text)).strip("-")


def location(s):
    where = f"{s['file']} › {s['key']}" if not s.get("section") else f"{s['file']} › [{s['section']}] › {s['key']}"
    return f"`{where}`"


def value_word(s, raw):
    if raw is None:
        return None
    if s["editor"] == "toggle":
        return "On" if str(raw).strip().lower() in ("true", "1", "on", "yes") else "Off"
    if raw == "":
        return "(empty)"
    for option in s.get("options") or []:
        value, _, label = option.partition("=")
        if value.strip().lower() == str(raw).strip().lower() and label:
            return f"`{value.strip()}` ({label.strip()})"
    return f"`{raw}`"


def allowed(s):
    lo, hi, unit = s.get("min"), s.get("max"), s.get("unit") or ""

    def number(n):
        return str(int(n)) if float(n).is_integer() else str(n)

    if s.get("options"):
        return "one of " + ", ".join(
            f"`{o.partition('=')[0].strip()}`" + (f" ({o.partition('=')[2].strip()})" if o.partition("=")[2] else "")
            for o in s["options"])
    if s["editor"] == "toggle":
        return "On or Off"
    if lo is not None and hi is not None:
        return f"{number(lo)} – {number(hi)}{' ' + unit if unit else ''}"
    if lo is not None:
        return f"{number(lo)} or more{' ' + unit if unit else ''}"
    if hi is not None:
        return f"at most {number(hi)}{' ' + unit if unit else ''}"
    if s.get("format") in FORMAT_WORDS:
        return FORMAT_WORDS[s["format"]]
    return None


def link(setting, by_id, group_names):
    """A link to another setting, on this page or another."""
    page = PAGE_NAMES[setting["category"]]
    heading = f"{setting['name']} ({setting['key']})"
    return f"[{setting['name']}]({page}#{anchor(heading)})"


def main():
    catalog = json.load(open(CATALOG, encoding="utf-8"))
    settings = catalog["settings"]
    by_id = {s["id"]: s for s in settings}
    categories = catalog["categories"]
    group_names = {g["id"]: g["name"] for c in categories for g in c["groups"]}
    for c in categories:
        PAGE_NAMES[c["id"]] = "Settings-" + slug(c["name"])

    # Reverse relations: what each setting controls / works with.
    controls = {s["id"]: [] for s in settings}
    for s in settings:
        for r in s.get("relations") or []:
            if r["id"] in controls:
                controls[r["id"]].append((s, r))

    written = []
    for c in categories:
        in_category = [s for s in settings if s["category"] == c["id"]]
        if not in_category:
            continue
        scope = "client" if c["scope"] == "client" else "server"
        lines = [
            f"# {c['name']} settings",
            "",
            f"{c['description']}".strip(),
            "",
            f"**{len(in_category)} settings** in the **{c['name']}** category of the "
            f"{'Client' if scope == 'client' else 'Server'} tab. "
            f"[All categories]({INDEX}) · [How to read this page]({INDEX}#how-to-read-these-pages)",
            "",
        ]
        for group in c["groups"]:
            in_group = [s for s in in_category if s["group"] == group["id"]]
            if not in_group:
                continue
            lines += [f"## {group['name']}", ""]
            for s in in_group:
                heading = f"{s['name']} ({s['key']})"
                lines.append(f"### {heading}")
                lines.append("")
                facts = [location(s), EDITOR_WORDS.get(s["editor"], s["editor"])]
                if s.get("unit"):
                    facts.append(s["unit"])
                if s.get("advanced"):
                    facts.append("*Advanced*")
                if s.get("l2everdream"):
                    facts.append("*added by L2Everdream*")
                lines.append(" · ".join(facts))
                lines.append("")
                if s.get("description"):
                    lines.append(s["description"].replace("\n", " ").strip())
                    lines.append("")
                bullets = []
                if s.get("default") is not None:
                    bullets.append(f"**Default:** {value_word(s, s['default'])}")
                if allowed(s):
                    bullets.append(f"**Allowed:** {allowed(s)}")
                if s.get("managedReason"):
                    bullets.append(f"**🔒 Set by the launcher:** {s['managedReason']}")
                for r in s.get("relations") or []:
                    other = by_id.get(r["id"])
                    if not other:
                        continue
                    if r["kind"] == "requires":
                        needed = {"true": "On", "false": "Off"}.get((r.get("value") or "").lower(), f"`{r.get('value')}`")
                        text = f"**Needs:** {link(other, by_id, group_names)} to be {needed}"
                    else:
                        text = f"**Works with:** {link(other, by_id, group_names)}"
                    if r.get("note"):
                        text += f" — {r['note']}"
                    bullets.append(text)
                for other, r in controls[s["id"]]:
                    if r["kind"] == "requires":
                        bullets.append(f"**Controls:** {link(other, by_id, group_names)} — it only works while this is "
                                       + {"true": "On", "false": "Off"}.get((r.get("value") or "").lower(), f"`{r.get('value')}`"))
                    else:
                        bullets.append(f"**Works with:** {link(other, by_id, group_names)}")
                lines += [f"- {b}" for b in bullets]
                lines.append("")
        page = PAGE_NAMES[c["id"]]
        with open(os.path.join(WIKI, page + ".md"), "w", encoding="utf-8", newline="\n") as fh:
            fh.write("\n".join(lines).rstrip() + "\n")
        written.append((c, page, len(in_category)))

    # Index page
    index = [
        "# Settings reference",
        "",
        f"Every setting the program can change: **{len(settings)}** in **{len(written)}** categories, generated from the same",
        "catalog the program itself uses, so the names, defaults and limits here are exactly what you see in the app. The value",
        "your own world uses may differ: the app shows it, and marks it *Changed* when it is not the default.",
        "",
        "Looking for one setting? The fastest way is the search box in the app, which matches friendly names, real setting",
        "names, file names and descriptions. In this wiki, use your browser's find (Ctrl+F) on a category page, or the wiki",
        "search box at the top of the page.",
        "",
        "## Server tab",
        "",
        "| Category | Settings | What it covers |",
        "|---|---|---|",
    ]
    for c, page, count in written:
        if c["scope"] != "client":
            index.append(f"| [{c['name']}]({page}) | {count} | {c['description']} |")
    index += ["", "## Client tab", "", "| Category | Settings | What it covers |", "|---|---|---|"]
    for c, page, count in written:
        if c["scope"] == "client":
            index.append(f"| [{c['name']}]({page}) | {count} | {c['description']} |")
    index += [
        "",
        "## How to read these pages",
        "",
        "Each setting looks like this:",
        "",
        "> ### Experience (XP) rate (RateXp)",
        "> `Rates.ini › RateXp` · number · times",
        ">",
        "> How fast characters gain experience from monsters. 1 = retail.",
        ">",
        "> - **Default:** `1`",
        "> - **Allowed:** 0 or more times",
        "> - **Works with:** Party experience rate",
        "",
        "| Line | Meaning |",
        "|---|---|",
        "| The heading | The friendly name in the app, and the real setting name in brackets |",
        "| The grey line | The file and key it is saved to, the kind of editor, its unit, and whether it is *Advanced* (hidden until you turn the Advanced filter on) or *added by L2Everdream* |",
        "| **Default** | What the setting is worth when nothing sets it |",
        "| **Allowed** | The values the program accepts. Anything else is refused when you save |",
        "| **🔒 Set by the launcher** | The launcher rewrites this on every start, so the app shows it locked |",
        "| **Needs** | This setting only does something while another setting has a certain value |",
        "| **Controls** | Other settings that only work while this one is set a certain way |",
        "| **Works with** | A related setting worth looking at together |",
        "",
        "Server settings take effect the next time you start your world from the launcher; client settings the next time you",
        "start Lineage 2. See [Server settings](Server-Settings) and [Client settings](Client-Settings) for how to find,",
        "change and save them.",
    ]
    with open(os.path.join(WIKI, INDEX + ".md"), "w", encoding="utf-8", newline="\n") as fh:
        fh.write("\n".join(index).rstrip() + "\n")

    print(f"{len(settings)} settings -> {len(written)} category pages + {INDEX}.md")
    for c, page, count in written:
        print(f"  {page}.md: {count}")


if __name__ == "__main__":
    main()
