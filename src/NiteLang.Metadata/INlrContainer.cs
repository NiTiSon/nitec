using System.Collections.Generic;

namespace NiteLang.Metadata;

public interface INlrContainer
{
	IEnumerable<INlrMember> Members { get; }
}