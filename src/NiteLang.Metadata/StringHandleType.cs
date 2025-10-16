namespace NiteLang.Metadata;

public enum StringTableBucketType : byte
{
	Utf8 = 0,
	Reserved1 = 0b01, // Utf16
	Reserved2 = 0b10, // Utf32
	Reserved3 = 0b11,
}