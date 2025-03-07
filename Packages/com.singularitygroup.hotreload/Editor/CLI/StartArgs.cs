namespace SingularityGroup.HotReload.Editor.Cli {
internal class StartArgs {
        public string hotreloadTempDir;
        // aka method patch temp dir
        public string cliTempDir;
        public string executableTargetDir;
        public string executableSourceDir;
        public string cliArguments;
        public string unityProjDir;
        public bool createNoWindow;
    }
}