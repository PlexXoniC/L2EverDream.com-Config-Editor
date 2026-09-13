"""Builds custom-config.json — every way L2Everdream's shipped server config differs from stock L2J Mobius.

Compares
  stock:    research/upstream-mobius/<commit>/{game,login}/config   (L2J_Mobius_CT_0_Interlude/dist, fetched from GitLab)
  shipped:  the L2Everdream release's own config, taken from the launcher's .shipped-baseline copies
            (…\\L2Everdream-data\\db\\config\\{game,login}\\.shipped-baseline) and, for files the launcher does not
            baseline (Custom\\*.ini, XML), from the install folder.

Output: catalog/custom-config.json, shown read-only in the app's Custom Config tab.
Run:    python catalog/build_custom_config.py <L2Everdream install folder>
"""
import difflib
import glob
import json
import os
import re
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.dirname(HERE)
sys.path.insert(0, HERE)
from build_catalog import scrub_names  # noqa: E402  (same no-names rule as the catalog)

ENTRY = re.compile(r"^\s*([A-Za-z0-9_.]+)\s*=\s*(.*?)\s*$")
TICKET = re.compile(r"\s*\[\[[A-Z]+-\d+\]\]|\s*\([A-Z]+-\d+[^)]*\)|[🔴⚠️⭐!]{1,2}\s*")
LAUNCHER_MANAGED = {"Database.ini", "ClassMaster.xml"}


def read(path):
    with open(path, encoding="utf-8", errors="replace") as fh:
        return fh.read()


def parse_ini(text):
    """key -> (value, comment block directly above it)."""
    result, comments = {}, []
    for line in text.splitlines():
        s = line.strip()
        if s.startswith("#"):
            comments.append(s.lstrip("#").strip())
            continue
        m = ENTRY.match(line)
        if m:
            result.setdefault(m.group(1), (m.group(2), comments))
        comments = [] if s == "" or m else comments
    return result


def owner_note(shipped_comments, stock_comments):
    """The explanation lines the release added above a setting, cleaned of names and ticket references."""
    stock = set(stock_comments)
    added = [c for c in shipped_comments if c not in stock and c and not re.fullmatch(r"-{3,}", c)]
    text = scrub_names("\n".join(added))
    text = TICKET.sub(" ", text)
    lines = [re.sub(r"\s{2,}", " ", l).strip() for l in text.splitlines()]
    return "\n".join(l for l in lines if l and not re.match(r"^(Default|Retail)\s*:", l, re.I))


def shipped_path(install, data, server, rel):
    baseline = os.path.join(data, "db", "config", server, ".shipped-baseline", rel)
    return baseline if os.path.exists(baseline) else os.path.join(install, server, "config", rel)


def main():
    install = os.path.abspath(sys.argv[1] if len(sys.argv) > 1 else os.path.join(os.environ["LOCALAPPDATA"], "L2Everdream"))
    data = os.path.join(os.path.dirname(install), "L2Everdream-data")
    stock_root = sorted(glob.glob(os.path.join(ROOT, "research", "upstream-mobius", "*")))[-1]
    source = json.load(open(os.path.join(stock_root, "SOURCE.json")))
    catalog = {(s["target"], s["file"], s["key"]): s for s in json.load(open(os.path.join(HERE, "catalog.json"), encoding="utf-8"))["settings"]}
    from build_catalog import load_tsv
    summaries = {(r["target"], r["file"], r["key"]): r["summary"] for r in load_tsv("custom-config-notes.tsv")}

    changes, files = [], []
    for server in ("game", "login"):
        stock_dir = os.path.join(stock_root, server, "config")
        install_dir = os.path.join(install, server, "config")
        names = {os.path.relpath(p, stock_dir).replace("\\", "/") for p in glob.glob(os.path.join(stock_dir, "**", "*"), recursive=True) if os.path.isfile(p)}
        names |= {os.path.relpath(p, install_dir).replace("\\", "/") for p in glob.glob(os.path.join(install_dir, "**", "*"), recursive=True) if os.path.isfile(p)}
        for rel in sorted(names):
            base = rel.split("/")[-1]
            if base in ("hexid.txt",) or rel.startswith("log"):
                continue
            stock_file = os.path.join(stock_dir, rel)
            ship_file = shipped_path(install, data, server, rel)
            target = f"server-{server}"
            if not os.path.exists(stock_file):
                files.append({"target": target, "file": rel, "kind": "added", "summary": "Not in stock L2J Mobius — added by L2Everdream."})
                continue
            if not os.path.exists(ship_file):
                files.append({"target": target, "file": rel, "kind": "removed", "summary": "Stock L2J Mobius file that L2Everdream does not ship."})
                continue
            stock_text, ship_text = read(stock_file), read(ship_file)
            if stock_text.replace("\r\n", "\n") == ship_text.replace("\r\n", "\n"):
                continue

            if not rel.endswith(".ini"):
                diff = [l for l in difflib.unified_diff(stock_text.splitlines(), ship_text.splitlines(), lineterm="", n=0)
                        if l[:1] in "+-" and not l.startswith(("+++", "---"))]
                added = sum(1 for l in diff if l.startswith("+"))
                removed = len(diff) - added
                files.append({
                    "target": target, "file": rel, "kind": "changed",
                    "summary": f"{added} line(s) added, {removed} removed compared with stock."
                               + (" The launcher also rewrites this file when the world starts." if base in LAUNCHER_MANAGED else ""),
                })
                continue

            stock, shipped = parse_ini(stock_text), parse_ini(ship_text)
            file_changes = 0
            for key in list(shipped) + [k for k in stock if k not in shipped]:
                s_val = stock.get(key, (None, []))
                h_val = shipped.get(key, (None, []))
                note = owner_note(h_val[1], s_val[1]) if key in shipped else ""
                if s_val[0] == h_val[0] and not note:
                    continue
                if key in stock and key in shipped:
                    kind = "value" if s_val[0] != h_val[0] else "note"
                else:
                    kind = "added" if key in shipped else "removed"
                if kind == "note" and base in LAUNCHER_MANAGED:
                    continue
                entry = catalog.get((target, rel, key))
                changes.append({
                    "target": target, "file": rel, "key": key, "kind": kind,
                    "name": entry["name"] if entry else key,
                    "settingId": entry["id"] if entry else None,
                    "category": entry["category"] if entry else None,
                    "stockValue": s_val[0], "shippedValue": h_val[0],
                    # Curated, neutral summary. The release's raw comments are internal notes and are not shown.
                    "note": summaries.get((target, rel, key), ""),
                    "launcherManaged": bool(entry and entry.get("managedReason")),
                })
                file_changes += 1
            if file_changes:
                files.append({"target": target, "file": rel, "kind": "settings", "summary": f"{file_changes} setting(s) differ from stock."})

    for c in changes:
        if not c["note"]:
            print(f"  no summary in custom-config-notes.tsv: {c['target']}|{c['file']}|{c['key']}")

    order = {"added": 0, "value": 1, "note": 2, "removed": 3}
    changes.sort(key=lambda c: (c["launcherManaged"], order[c["kind"]], c["file"], c["key"]))
    out = {
        "version": 1,
        "stock": {"project": source["project"], "path": source["path"], "commit": source["commit"][:12], "committed": source["committed"][:10]},
        "changes": changes,
        "files": files,
    }
    with open(os.path.join(HERE, "custom-config.json"), "w", encoding="utf-8") as fh:
        json.dump(out, fh, indent=1, ensure_ascii=False)
    kinds = {}
    for c in changes:
        kinds[c["kind"]] = kinds.get(c["kind"], 0) + 1
    print(f"{len(changes)} setting differences {kinds}; {len(files)} files listed")


if __name__ == "__main__":
    main()
