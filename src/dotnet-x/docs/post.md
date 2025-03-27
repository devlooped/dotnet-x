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
