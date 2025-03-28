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
      dotnet tool update -g dotnet-xcli 
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
