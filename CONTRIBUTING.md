# Contributing

## Setup

### Prerequisites

- [.NET Core SDK 5.0.300](https://dotnet.microsoft.com/download/dotnet)

### Install

1. Fork the project, then clone it:
```
git clone git@github.com:flyingpie/windows-terminal-quake.git
```
2. Open the project in your IDE
3. Run `./Build.ps1`

### Troubleshooting

##### SDK not installed

######  Error:

`Could not execute because the application was not found or a compatible .NET SDK is not installed.`

###### Solution:

Make sure you restart your terminal.

##### SDK specified could not be found

###### Error:

`Error MSB4236: The SDK 'Microsoft.NET.Sdk' specified could not be found.` 

###### Solution: 

Set the the following environment variable

```PS
$env:MSBuildSDKsPath = "C:\Program Files\dotnet\sdk\5.0.300\Sdks"
```

## Proposing changes

1. Create a branch

```
git checkout -b my-feature
```

2. Make your changes and update the readme if necessary.
3. Test if everything works as expected.
4. Push your code

```
git push -u origin my-feature
```

5. Draft a pull request by clicking on the link or from the GitHub interface.
6. In the pull request, describe what your changes intend to do and why they are needed.