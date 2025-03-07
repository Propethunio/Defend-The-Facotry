using System;

namespace SingularityGroup.HotReload.Editor {
internal interface ICompileChecker {
        event Action onCompilationFinished;
        bool hasCompileErrors { get; }
    }

internal static class CompileChecker {
        internal static ICompileChecker Create() {
            #if UNITY_2019_1_OR_NEWER
                return new DefaultCompileChecker();
            #else
                return new LegacyCompileChecker();
            #endif
        }
    }
}