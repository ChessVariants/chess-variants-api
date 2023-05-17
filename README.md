# ChessVariantsAPI

## Setup
To run the environment locally you need the .NET Dependencies, MongoDB and a few user secrets.

# Setup .NET

1. Download Visual Studio (VS):
   - Windows -> https://visualstudio.microsoft.com/vs/
   - Mac -> https://visualstudio.microsoft.com/vs/mac/
   - Linux -> Use VS Code instead, or JetBrains Rider.
2. Download .NET 6 Dependencies (SDK, .NET Runtime, ASP.NET Core Runtime), either through VS installation process (easiest) or [separately](https://learn.microsoft.com/en-us/dotnet/core/install/linux) if running linux
3. Clone this repo to your desired directory.
4. Download project dependencies, VS does this automatically but it can also be done via the [dotnet CLI](https://learn.microsoft.com/en-us/nuget/consume-packages/install-use-packages-dotnet-cli)
5. Open solution if using VS
6. Finished!

# Setup MongoDB and user secrets
1. Install MongoDB locally, or use a free hosting tier with Mongo Atlas. https://www.mongodb.com/docs/manual/installation/
   - If using the Atlas solution, make sure your IP-address is added to the IP access list.
2. Run the following commands in the ChessVariantsAPI project:
   - `dotnet user-secrets init`
   - `dotnet user-secrets set "MongoDatabase:ConnectionString" "your MongoDB connection string"`
   - `dotnet user-secrets set "Authentication:JWTSecret" "your secret string, can be anything u like"`

# Running the program
1. Run the ChessVariantsAPI project
2. Start the [frontend](https://github.com/ChessVariants/chess-variants-frontend)

This will allow you to play all the standard variants. However, the custom variants utilize standard pieces stored in the database. As your database will be empty from scratch that will unfortunately not work. A script to populate the database with necessary data for custom variants might be added at a later point.
