# WinGet release
1. Build using "build.ps1"
2. Calculate SHA256 hash for self-contained version: ```certutil -hashfile win-x64_self-contained.zip SHA256```
3. Create manifest for new version.
4. Test new version: ```.\Tools\SandboxTest.ps1 .\manifests\f\flyingpie\windows-terminal-quake\prerelease\2.0.0.6\```
