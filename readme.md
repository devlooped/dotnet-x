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
[![Clarius Org](https://avatars.githubusercontent.com/u/71888636?v=4&s=39 "Clarius Org")](https://github.com/clarius)
[![MFB Technologies, Inc.](https://avatars.githubusercontent.com/u/87181630?v=4&s=39 "MFB Technologies, Inc.")](https://github.com/MFB-Technologies-Inc)
[![Khamza Davletov](https://avatars.githubusercontent.com/u/13615108?u=11b0038e255cdf9d1940fbb9ae9d1d57115697ab&v=4&s=39 "Khamza Davletov")](https://github.com/khamza85)
[![SandRock](https://avatars.githubusercontent.com/u/321868?u=99e50a714276c43ae820632f1da88cb71632ec97&v=4&s=39 "SandRock")](https://github.com/sandrock)
[![DRIVE.NET, Inc.](https://avatars.githubusercontent.com/u/15047123?v=4&s=39 "DRIVE.NET, Inc.")](https://github.com/drivenet)
[![Keith Pickford](https://avatars.githubusercontent.com/u/16598898?u=64416b80caf7092a885f60bb31612270bffc9598&v=4&s=39 "Keith Pickford")](https://github.com/Keflon)
[![Thomas Bolon](https://avatars.githubusercontent.com/u/127185?u=7f50babfc888675e37feb80851a4e9708f573386&v=4&s=39 "Thomas Bolon")](https://github.com/tbolon)
[![Kori Francis](https://avatars.githubusercontent.com/u/67574?u=3991fb983e1c399edf39aebc00a9f9cd425703bd&v=4&s=39 "Kori Francis")](https://github.com/kfrancis)
[![Reuben Swartz](https://avatars.githubusercontent.com/u/724704?u=2076fe336f9f6ad678009f1595cbea434b0c5a41&v=4&s=39 "Reuben Swartz")](https://github.com/rbnswartz)
[![Jacob Foshee](https://avatars.githubusercontent.com/u/480334?v=4&s=39 "Jacob Foshee")](https://github.com/jfoshee)
[![](https://avatars.githubusercontent.com/u/33566379?u=bf62e2b46435a267fa246a64537870fd2449410f&v=4&s=39 "")](https://github.com/Mrxx99)
[![Eric Johnson](https://avatars.githubusercontent.com/u/26369281?u=41b560c2bc493149b32d384b960e0948c78767ab&v=4&s=39 "Eric Johnson")](https://github.com/eajhnsn1)
[![Jonathan ](https://avatars.githubusercontent.com/u/5510103?u=98dcfbef3f32de629d30f1f418a095bf09e14891&v=4&s=39 "Jonathan ")](https://github.com/Jonathan-Hickey)
[![Ken Bonny](https://avatars.githubusercontent.com/u/6417376?u=569af445b6f387917029ffb5129e9cf9f6f68421&v=4&s=39 "Ken Bonny")](https://github.com/KenBonny)
[![Simon Cropp](https://avatars.githubusercontent.com/u/122666?v=4&s=39 "Simon Cropp")](https://github.com/SimonCropp)
[![agileworks-eu](https://avatars.githubusercontent.com/u/5989304?v=4&s=39 "agileworks-eu")](https://github.com/agileworks-eu)
[![Zheyu Shen](https://avatars.githubusercontent.com/u/4067473?v=4&s=39 "Zheyu Shen")](https://github.com/arsdragonfly)
[![Vezel](https://avatars.githubusercontent.com/u/87844133?v=4&s=39 "Vezel")](https://github.com/vezel-dev)
[![ChilliCream](https://avatars.githubusercontent.com/u/16239022?v=4&s=39 "ChilliCream")](https://github.com/ChilliCream)
[![4OTC](https://avatars.githubusercontent.com/u/68428092?v=4&s=39 "4OTC")](https://github.com/4OTC)
[![domischell](https://avatars.githubusercontent.com/u/66068846?u=0a5c5e2e7d90f15ea657bc660f175605935c5bea&v=4&s=39 "domischell")](https://github.com/DominicSchell)
[![Adrian Alonso](https://avatars.githubusercontent.com/u/2027083?u=129cf516d99f5cb2fd0f4a0787a069f3446b7522&v=4&s=39 "Adrian Alonso")](https://github.com/adalon)
[![torutek](https://avatars.githubusercontent.com/u/33917059?v=4&s=39 "torutek")](https://github.com/torutek)
[![mccaffers](https://avatars.githubusercontent.com/u/16667079?u=110034edf51097a5ee82cb6a94ae5483568e3469&v=4&s=39 "mccaffers")](https://github.com/mccaffers)
[![Seika Logiciel](https://avatars.githubusercontent.com/u/2564602?v=4&s=39 "Seika Logiciel")](https://github.com/SeikaLogiciel)
[![Andrew Grant](https://avatars.githubusercontent.com/devlooped-user?s=39 "Andrew Grant")](https://github.com/wizardness)


<!-- sponsors.md -->
[![Sponsor this project](https://avatars.githubusercontent.com/devlooped-sponsor?s=118 "Sponsor this project")](https://github.com/sponsors/devlooped)

[Learn more about GitHub Sponsors](https://github.com/sponsors)

<!-- https://github.com/devlooped/sponsors/raw/main/footer.md -->
