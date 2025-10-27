# Creating a WinGet release

*Replace 2.0.8 with the actual version.*

## Create Release

```ps1
./src/build.ps1 --target PublishRelease --github-token <GH_TOKEN> --github-release v2.0.8 --sem-ver-version 2.0.8
```

## Prepare Manifest PR
- Clone the [WinGet manifest repo](https://github.com/microsoft/winget-pkgs).
- Copy ```~/winget/2.0.8/``` to ```~/manifests/f/flyingpie/windows-terminal-quake/2.0.8/```

## Test Release
- Get [SandboxTest.ps1](https://raw.githubusercontent.com/microsoft/winget-pkgs/refs/heads/master/Tools/SandboxTest.ps1).
- Test in sandbox:
```ps1
./SandboxTest.ps1 -Manifest winget-pkgs\manifests\f\flyingpie\windows-terminal-quake\2.0.8\
```

## Create PR
And wait.
