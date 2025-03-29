![Icon](assets/img/logo.png) A CLI for X
============

[![Version](https://img.shields.io/nuget/vpre/dotnet-x.svg?color=royalblue)](https://www.nuget.org/packages/dotnet-x)
[![Downloads](https://img.shields.io/nuget/dt/dotnet-x.svg?color=green)](https://www.nuget.org/packages/dotnet-x)
[![License](https://img.shields.io/github/license/devlooped/dotnet-x.svg?color=blue)](https://github.com//devlooped/dotnet-x/blob/main/license.txt)
[![Build](https://github.com/devlooped/dotnet-x/actions/workflows/build.yml/badge.svg?branch=main)](https://github.com/devlooped/dotnet-x/actions/workflows/build.yml)

<!-- #content -->

This CLI allows you to manage your personal posts from the command line. 
It requires creating a project and app on the [X Developer Portal](https://developer.x.com/en/portal/dashboard) to generate the necessary API keys and secrets.

## Usage

<!-- include src/dotnet-x/docs/help.md -->
```shell
> x --help
USAGE:
    x [OPTIONS] <COMMAND>

EXAMPLES:
    x post "Hello, world!" --media path/to/image.png

OPTIONS:
    -h, --help    Prints help information

COMMANDS:
    auth            
    post <TEXT>     
```

<!-- src/dotnet-x/docs/help.md -->

## Authentication

Authentication is managed for you by the CLI, using the [Git Credential Manager](https://github.com/git-ecosystem/git-credential-manager) 
as the cross-platform secure storage for your API key(s). You can login multiple project/key 
combination and then just change the active one without ever re-entering the keys.

See [getting access on X](https://docs.x.com/x-api/getting-started/getting-access) for more details.

<!-- include src/dotnet-x/docs/auth-login.md -->
```shell
> x auth login --help
DESCRIPTION:
Authenticate to X by providing the required secrets. 

Supports API key autentication using the Git Credential Manager for storage.

Switch easily between keys by just specifying an alias for the keys.

Alternatively, x will use the secrets found in environment variables with the 
prefix `X_`: `X_AccessToken`, `X_AccessTokenSecret`, `X_ConsumerKey`, 
`X_ConsumerSecret`.
Using double underscores also works for nested configuration, such as 
`X__ConsumerKey`.
This method is most suitable for "headless" use such as in automation.

For example, to use x in GitHub Actions:
  - name: ✖️ post
    env:
      X_AccessToken: ${{ secrets.X_ACCESS_TOKEN }}
      X_AccessTokenSecret: ${{ secrets.X_ACCESS_TOKEN_SECRET }}
      X_ConsumerKey: ${{ secrets.X_CONSUMER_KEY }}
      X_ConsumerSecret: ${{ secrets.X_CONSUMER_SECRET }}
    run: |
      dotnet tool update -g dotnet-x 
      x post "Hello, world!" --media image.png

USAGE:
    x auth login <alias> [OPTIONS]

ARGUMENTS:
    <alias>    Alias to use for the set of credentials

OPTIONS:
    -h, --help    Prints help information                              
        --at      Access token. Required unless previously saved       
        --ats     Access token secret. Required unless previously saved
        --ck      Consumer key. Required unless previously saved       
        --cs      Consumer secret. Required unless previously saved    
```

<!-- src/dotnet-x/docs/auth-login.md -->

<!-- include src/dotnet-x/docs/auth-logout.md -->
```shell
> x auth logout --help
DESCRIPTION:
Log out of X

USAGE:
    x auth logout [alias] [OPTIONS]

ARGUMENTS:
    [alias]    Specific alias to log out. Removes all accounts if not provided

OPTIONS:
    -h, --help    Prints help information
```

<!-- src/dotnet-x/docs/auth-logout.md -->

<!-- include src/dotnet-x/docs/auth-status.md -->
```shell
> x auth status --help
DESCRIPTION:
Shows the current authentication status

USAGE:
    x auth status [OPTIONS]

OPTIONS:
    -h, --help            Prints help information
        --show-secrets    Display the secrets    
```

<!-- src/dotnet-x/docs/auth-status.md -->

## Posting

<!-- include src/dotnet-x/docs/post.md -->
```shell
> x post --help
USAGE:
    x post <TEXT> [OPTIONS]

EXAMPLES:
    x post "Hello, world!" --media path/to/image.png

ARGUMENTS:
    <TEXT>    Text to post

OPTIONS:
    -h, --help               Prints help information                            
        --jq [EXPRESSION]    Filter JSON output using a jq expression           
        --json               Output as JSON. Implied when using --jq            
        --monochrome         Disable colors when rendering JSON to the console  
    -m, --media <MEDIA>      Zero or more media files to attach to the post     
                             (.jpg, .jpeg, .png, .gif, .webp, .mp4, .mov)       
```

<!-- src/dotnet-x/docs/post.md -->

<!-- #content -->
<!-- include https://github.com/devlooped/sponsors/raw/main/footer.md -->
# Sponsors 

<!-- sponsors.md -->
[![Clarius Org](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/clarius.png "Clarius Org")](https://github.com/clarius)
[![MFB Technologies, Inc.](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/MFB-Technologies-Inc.png "MFB Technologies, Inc.")](https://github.com/MFB-Technologies-Inc)
[![Torutek](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/torutek-gh.png "Torutek")](https://github.com/torutek-gh)
[![DRIVE.NET, Inc.](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/drivenet.png "DRIVE.NET, Inc.")](https://github.com/drivenet)
[![Keith Pickford](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/Keflon.png "Keith Pickford")](https://github.com/Keflon)
[![Thomas Bolon](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/tbolon.png "Thomas Bolon")](https://github.com/tbolon)
[![Kori Francis](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/kfrancis.png "Kori Francis")](https://github.com/kfrancis)
[![Toni Wenzel](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/twenzel.png "Toni Wenzel")](https://github.com/twenzel)
[![Uno Platform](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/unoplatform.png "Uno Platform")](https://github.com/unoplatform)
[![Dan Siegel](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/dansiegel.png "Dan Siegel")](https://github.com/dansiegel)
[![Reuben Swartz](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/rbnswartz.png "Reuben Swartz")](https://github.com/rbnswartz)
[![Jacob Foshee](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/jfoshee.png "Jacob Foshee")](https://github.com/jfoshee)
[![](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/Mrxx99.png "")](https://github.com/Mrxx99)
[![Eric Johnson](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/eajhnsn1.png "Eric Johnson")](https://github.com/eajhnsn1)
[![Ix Technologies B.V.](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/IxTechnologies.png "Ix Technologies B.V.")](https://github.com/IxTechnologies)
[![David JENNI](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/davidjenni.png "David JENNI")](https://github.com/davidjenni)
[![Jonathan ](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/Jonathan-Hickey.png "Jonathan ")](https://github.com/Jonathan-Hickey)
[![Charley Wu](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/akunzai.png "Charley Wu")](https://github.com/akunzai)
[![Jakob Tikjøb Andersen](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/jakobt.png "Jakob Tikjøb Andersen")](https://github.com/jakobt)
[![Ken Bonny](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/KenBonny.png "Ken Bonny")](https://github.com/KenBonny)
[![Simon Cropp](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/SimonCropp.png "Simon Cropp")](https://github.com/SimonCropp)
[![agileworks-eu](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/agileworks-eu.png "agileworks-eu")](https://github.com/agileworks-eu)
[![sorahex](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/sorahex.png "sorahex")](https://github.com/sorahex)
[![Zheyu Shen](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/arsdragonfly.png "Zheyu Shen")](https://github.com/arsdragonfly)
[![Vezel](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/vezel-dev.png "Vezel")](https://github.com/vezel-dev)
[![ChilliCream](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/ChilliCream.png "ChilliCream")](https://github.com/ChilliCream)
[![4OTC](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/4OTC.png "4OTC")](https://github.com/4OTC)
[![Vincent Limo](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/v-limo.png "Vincent Limo")](https://github.com/v-limo)
[![Jordan S. Jones](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/jordansjones.png "Jordan S. Jones")](https://github.com/jordansjones)
[![domischell](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/DominicSchell.png "domischell")](https://github.com/DominicSchell)


<!-- sponsors.md -->

[![Sponsor this project](https://raw.githubusercontent.com/devlooped/sponsors/main/sponsor.png "Sponsor this project")](https://github.com/sponsors/devlooped)
&nbsp;

[Learn more about GitHub Sponsors](https://github.com/sponsors)

<!-- https://github.com/devlooped/sponsors/raw/main/footer.md -->
