using System;
using System.Linq;

namespace Nlr.Compiler.CodeAnalysis.NiteCode;

public sealed class NiteCodeOptions
{
	public LanguageVersion Version { get; }

	public NiteCodeOptions(LanguageVersion version)
	{
		if (version == LanguageVersion.Latest)
		{
			version = Enum.GetValues<LanguageVersion>().SkipLast(1).Last();
		}

		Version = version;
	}
}