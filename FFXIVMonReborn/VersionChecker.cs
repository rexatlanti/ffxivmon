using System.Net;
using FFXIVMonReborn.Database.Commits;

namespace FFXIVMonReborn
{
    public static class VersionChecker
    {
        private const string Repo = "SapphireMordred/ffxivmon";

        public static void CheckVersion()
        {
            #if DEBUG
            return;
            #endif
            
            var currentHash = Util.GetGitHash();

            // If this is a working copy, don't alert about new versions
            if (currentHash.Contains("dirty"))
                return;

            var newCommit = GetNewestCommit();
            
            if (!newCommit.Sha.StartsWith(currentHash))
            {
                new ExtendedErrorView(
                    "There is a new version available. Please check out new changes from the github repo.",
                    $"New Version: {newCommit.Sha}\n\n{newCommit.Commit.Message}\nBy: {newCommit.Commit.Author.Name}", "New Version available").ShowDialog();
            }    
        }

        public static GithubApiCommits GetNewestCommit()
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("User-Agent", "XIVMon");
                var result =
                    client.DownloadString($"https://api.github.com/repos/{Repo}/commits");
                
                return GithubApiCommits.FromJson(result)[0];
            }
        }
    }
}