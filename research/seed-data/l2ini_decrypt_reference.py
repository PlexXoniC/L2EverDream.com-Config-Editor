"""Reference decoder for Lineage 2 client ini files (port to C# for the config manager).

Lineage2Ver413 (l2.ini, user.ini): 28-byte UTF-16LE header "Lineage2Ver413", then 128-byte RSA
blocks, then a 20-byte tail (CRC32 at tail offset 12). Each block decrypts (c^e mod n) to 128 bytes;
byte 3 holds the payload size, the payload sits right-aligned before (-size & 3) padding bytes.
Joined payload = 4-byte LE uncompressed length + zlib stream. Key constants match the launcher's
org.l2sp.launcher.world.L2IniCrypt (which also encrypts, with the private exponent it carries).

Lineage2Ver111 (Localization.ini, TTFontInfo.ini): 28-byte header, then every byte XOR 0xAC (ANSI text).

Files with no header (Option.ini, WindowsInfo.ini, chatfilter.ini, s_info.ini) are plain text.
"""
import struct
import sys
import zlib

MODULUS = int(
    "75b4d6de5c016544068a1acf125869f43d2e09fc55b8b1e289556daf9b8757635593446288b3653da1ce91c87bb1a5c1"
    "8f16323495c55d7d72c0890a83f69bfd1fd9434eb1c02f3e4679edfa43309319070129c267c85604d87bb65bae205de3"
    "707af1d2108881abb567c3b3d069ae67c3a4c6a3aa93d26413d4c66094ae2039", 16)
PUBLIC_EXPONENT = 0x1D
HEADER = 28
TAIL = 20


def header_version(data: bytes) -> str | None:
    text = data[:HEADER].decode("utf-16-le", "replace")
    return text[len("Lineage2Ver"):] if text.startswith("Lineage2Ver") else None


def decode(data: bytes) -> str:
    version = header_version(data)
    if version is None:
        return data.decode("latin-1")
    if version == "111":
        return bytes(b ^ 0xAC for b in data[HEADER:]).decode("latin-1")
    if version != "413":
        raise ValueError(f"unsupported Lineage2Ver{version}")
    body = data[HEADER:-TAIL]
    payload = bytearray()
    for i in range(0, len(body), 128):
        block = pow(int.from_bytes(body[i:i + 128], "big"), PUBLIC_EXPONENT, MODULUS).to_bytes(128, "big")
        size = block[3]
        pad = (-size) & 3
        payload += block[128 - size - pad:128 - pad]
    expected = struct.unpack("<I", payload[:4])[0]
    raw = zlib.decompress(bytes(payload[4:]))
    if len(raw) != expected:
        raise ValueError(f"length mismatch: {len(raw)} != {expected}")
    return raw.decode("utf-16-le") if raw[1:2] == b"\x00" else raw.decode("latin-1")


if __name__ == "__main__":
    sys.stdout.reconfigure(encoding="utf-8")
    print(decode(open(sys.argv[1], "rb").read()))
