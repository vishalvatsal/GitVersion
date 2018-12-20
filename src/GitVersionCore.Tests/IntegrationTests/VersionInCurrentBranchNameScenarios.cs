namespace GitVersionCore.Tests.IntegrationTests
{
    using System.Collections.Generic;
    using System.Linq;

    using GitTools.Testing;

    using GitVersion;

    using GitVersionCore.Tests;

    using LibGit2Sharp;

    using NUnit.Framework;

    [TestFixture]
    public class VersionInCurrentBranchNameScenarios : TestBase
    {
        [Test]
        public void TakesVersionFromNameOfReleaseBranch()
        {
            using (var fixture = new BaseGitFlowRepositoryFixture("1.0.0"))
            {
                fixture.BranchTo("release/2.0.0");

                fixture.AssertFullSemver("2.0.0-beta.1+0");
            }
        }

        [Test]
        public void DoesNotTakeVersionFromNameOfNonReleaseBranch()
        {
            using (var fixture = new BaseGitFlowRepositoryFixture("1.0.0"))
            {
                fixture.BranchTo("feature/upgrade-power-level-to-9000.0.1");

                fixture.AssertFullSemver("1.1.0-upgrade-power-level-to-9000-0-1.1+1");
            }
        }

        [Test]
        public void TakesVersionFromNameOfBranchThatIsReleaseByConfig()
        {
            var config = new Config
            {
                Branches = new Dictionary<string, BranchConfig> { { "support", new BranchConfig { IsReleaseBranch = true } } }
            };

            using (var fixture = new BaseGitFlowRepositoryFixture("1.0.0"))
            {
                fixture.BranchTo("support/2.0.0");

                fixture.AssertFullSemver(config, "2.0.0+1");
            }
        }

        [Test]
        public void TakesVersionFromNameOfRemoteReleaseBranch()
        {
            using (var fixture = new RemoteRepositoryFixture())
            {
                fixture.BranchTo("release/2.0.0");
                fixture.MakeACommit();
                Commands.Fetch((Repository)fixture.LocalRepositoryFixture.Repository, fixture.LocalRepositoryFixture.Repository.Network.Remotes.First().Name, new string[0], new FetchOptions(), null);

                fixture.LocalRepositoryFixture.Checkout("origin/release/2.0.0");

                fixture.LocalRepositoryFixture.AssertFullSemver("2.0.0-beta.1+1");
            }
        }
    }
}
