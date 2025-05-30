using Nlr.Compiler;
using System.Threading.Tasks;

namespace Nlr.Tests;

public class SemVerTests
{
	private static readonly string[] ValidSemVersions = [
		"0.0.4",
		"1.2.3",
		"10.20.30",
		"1.1.2-prerelease+meta",
		"1.1.2+meta",
		"1.1.2+meta-valid",
		"1.0.0-alpha",
		"1.0.0-beta",
		"1.0.0-alpha.beta",
		"1.0.0-alpha.beta.1",
		"1.0.0-alpha.1",
		"1.0.0-alpha0.valid",
		"1.0.0-alpha.0valid",
		"1.0.0-alpha-a.b-c-somethinglong+build.1-aef.1-its-okay",
		"1.0.0-rc.1+build.1",
		"2.0.0-rc.1+build.123",
		"1.2.3-beta",
		"10.2.3-DEV-SNAPSHOT",
		"1.2.3-SNAPSHOT-123",
		"1.0.0",
		"2.0.0",
		"1.1.7",
		"2.0.0+build.1848",
		"2.0.1-alpha.1227",
		"1.0.0-alpha+beta",
		"1.2.3----RC-SNAPSHOT.12.9.1--.12+788",
		"1.2.3----R-S.12.9.1--.12+meta",
		"1.2.3----RC-SNAPSHOT.12.9.1--.12",
		"1.0.0+0.build.1-rc.10000aaa-kk-0.1",
		"1.0.0-0A.is.legal",
	];
	
	private static readonly string[] InvalidSemVersions = [
		"1",
		"1.2",
		"1.2.3-0123",
		"1.2.3-0123.0123",
		"1.1.2+.123",
		"+invalid",
		"-invalid",
		"-invalid+invalid",
		"-invalid.01",
		"alpha",
		"alpha.beta",
		"alpha.beta.1",
		"alpha.1",
		"alpha+beta",
		"alpha_beta",
		"alpha.",
		"alpha..",
		"beta",
		"1.0.0-alpha_beta",
		"-alpha.",
		"1.0.0-alpha..",
		"1.0.0-alpha..1",
		"1.0.0-alpha...1",
		"1.0.0-alpha....1",
		"1.0.0-alpha.....1",
		"1.0.0-alpha......1",
		"1.0.0-alpha.......1",
		"01.1.1",
		"1.01.1",
		"1.1.01",
		"1.2",
		"1.2.3.DEV",
		"1.2-SNAPSHOT",
		"1.2.31.2.3----RC-SNAPSHOT.12.09.1--..12+788",
		"1.2-RC-SNAPSHOT",
		"-1.0.3-gamma+b7718",
		"+justmeta",
		"9.8.7+meta+meta",
		"9.8.7-whatever+meta+meta",
		"99999999999999999999999.999999999999999999.99999999999999999",
		"99999999999999999999999.999999999999999999.99999999999999999----RC-SNAPSHOT.12.09.1--------------------------------..12",
	];
    
    [Test]
    public async Task SemVer_TryParse_ShallSuccess()
    {
	    foreach (string semVer in ValidSemVersions)
	    {
		    bool result = SemVer.TryParse(semVer, out _);
		    await Assert.That(result).IsTrue().Because(semVer);
	    }
    }
    
    [Test]
    public async Task SemVer_TryParse_ShallFail()
    {
	    foreach (string semVer in InvalidSemVersions)
	    {
		    bool result = SemVer.TryParse(semVer, out _);
		    await Assert.That(result).IsFalse().Because(semVer);
	    }
    }

    [Test]
    public async Task SemVer_ToString()
    {
	    foreach (string semVersion in ValidSemVersions)
	    {
		    bool result = SemVer.TryParse(semVersion, out SemVer ver);
		    
		    await Assert.That(result).IsTrue().Because(semVersion);
		    
			await Assert.That(ver.ToString()).IsEqualTo(semVersion);
	    }
    }

    private record struct TestData(string SemVer, string[] Identifiers);
    private static readonly TestData[] PrereleaseTestData =
    [
	    new("1.0.0", []),
	    new("1.0.0-1.2", ["1", "2"]),
	    new("1.0.0-1.2.3", ["1", "2", "3"]),
	    new("1.0.0-123", ["123"]),
	    new("1.0.0-rc", ["rc"]),
	    new("1.0.0-rc.12", ["rc", "12"]),
	    new("1.0.0-rc.1+build.1", ["rc", "1"]),
    ];
    private static readonly TestData[] MetaTestData =
    [
	    new("1.0.0", []),
	    new("1.0.0+1.2", ["1", "2"]),
	    new("1.0.0+1.2.3", ["1", "2", "3"]),
	    new("1.0.0+123", ["123"]),
	    new("1.0.0+rc", ["rc"]),
	    new("1.0.0+rc.12", ["rc", "12"]),
	    new("1.0.0-rc.1+build.1", ["build", "1"]),
    ];
    
    [Test]
    public async Task SemVer_PrereleaseIdentifiers()
    {
	    foreach (TestData identifier in PrereleaseTestData)
	    {
		    SemVer semVer = SemVer.Parse(identifier.SemVer);

		    using (Assert.Multiple())
		    {
			    await Assert.That(semVer.PrereleaseIdentifiers.Count).IsEqualTo(identifier.Identifiers.Length);
			    
			    await Assert.That(semVer.PrereleaseIdentifiers).IsEquivalentTo(identifier.Identifiers);
		    }
	    }
    }
    
    [Test]
    public async Task SemVer_MetaIdentifiers()
    {
	    foreach (TestData identifier in MetaTestData)
	    {
		    SemVer semVer = SemVer.Parse(identifier.SemVer);

		    using (Assert.Multiple())
		    {
			    await Assert.That(semVer.MetaIdentifiers.Count).IsEqualTo(identifier.Identifiers.Length);
			    
			    await Assert.That(semVer.MetaIdentifiers).IsEquivalentTo(identifier.Identifiers);
		    }
	    }
    }

    [Test]
    public async Task SemVer_Equals()
    {
	    SemVer v1_0_0 = new(1, 0, 0);
	    SemVer v1_0_1 = new(1, 0, 1);
	    SemVer v1_0_0withMeta = SemVer.Parse("1.0.0+meta");
	    SemVer v1_0_0withPrerelease = SemVer.Parse("1.0.0-prerelease");

	    using (Assert.Multiple())
	    {
		    await Assert.That(v1_0_0).IsNotEqualTo(v1_0_1);
		    await Assert.That(v1_0_0).IsEqualTo(v1_0_0withMeta);
		    await Assert.That(v1_0_0).IsGreaterThan(v1_0_0withPrerelease);
		    await Assert.That(v1_0_0).IsLessThan(v1_0_1);
	    }
    }
}