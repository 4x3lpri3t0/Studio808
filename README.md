## Table of Contents

- [API Definition](#api-definition)
  * [Create User](#create-user)
    + [Response](#response)
  * [Save Game State](#save-game-state)
  * [Load Game State](#load-game-state)
    + [Response](#response-1)
  * [Update Friends](#update-friends)
  * [Get Friends](#get-friends)
    + [Response](#response-2)
  * [Debug: Get All Users](#debug--get-all-users)
    + [Response](#response-3)
- [Execution & Testing](#execution---testing)
- [Assumptions, Suggestions, Improvements](#assumptions--suggestions--improvements)

## API Definition

### Create User

When the game is started up the first (or reset) time it will create a new user. The game expects this call to return a user id.

```json
POST /user
{
  "name": "John"
}
```

#### Response

```json
{
  "id": "18dd75e9-3d4a-48e2-bafc-3c8f95a8f0d1",
  "name": "John"
}
```

### Save Game State

Whenever a game is played it will try to save its game state using this request.

```json
PUT /user/<userid>/state
{
  "gamesPlayed": 42,
  "score": 358
}
```

### Load Game State

On starting up the game it will load the game state. The score field should be considered to be the player's current highscore.

```json
GET /user/<userid>/state
```

#### Response

```json
{
  "gamesPlayed": 42,
  "score": 358
}
```

### Update Friends

This request will only happen when the friend list actually changes. The request contains the full list of friends.

```json
PUT /user/<userid>/friends
{
  "friends": [
    "18dd75e9-3d4a-48e2-bafc-3c8f95a8f0d1",
    "f9a9af78-6681-4d7d-8ae7-fc41e7a24d08",
    "2d18862b-b9c3-40f5-803e-5e100a520249"
  ]
}
```

### Get Friends

This request is called upon starting the game and must return the games list of friends as well as their high scores (latest score as listed in the game state) to allow populating the high-score table.

```json
GET /user/<userid>/friends
```

#### Response

```json
{
  "friends":[
    {
      "id":"18dd75e9-3d4a-48e2-bafc-3c8f95a8f0d1",
      "name":"John",
      "highscore":322
    },
    {
      "id":"f9a9af78-6681-4d7d-8ae7-fc41e7a24d08",
      "name":"Bob",
      "highscore":21
    },
    {
      "id":"2d18862b-b9c3-40f5-803e-5e100a520249",
      "name":"Alice",
      "highscore":99332
    }
  ]
}
```

### Debug: Get All Users

For now the game uses this request to get all users. Player can select who of these that he wants to be friends with. Obviously this will not scale at all; we will fix this later. For now just implement it as is.

```json
GET /user
```

#### Response

```json
{
  "users":[
    {
      "id":"18dd75e9-3d4a-48e2-bafc-3c8f95a8f0d1",
      "name":"John"
    },
    {
      "id":"f9a9af78-6681-4d7d-8ae7-fc41e7a24d08",
      "name":"Bob"
    },
    {
      "id":"2d18862b-b9c3-40f5-803e-5e100a520249",
      "name":"Alice"
    }
  ]
}
```

## Execution & Testing

- First of all, I recommend you make sure you have the latest [.NET Core SDK, available here](https://dotnet.microsoft.com/download)
- Load up the solution (`SYBO-Exercise.sln`) from either IDE:
  - [Visual Studio Code](https://code.visualstudio.com/)
  - [Visual Studio Community](https://visualstudio.microsoft.com/vs/community/)
- You can Compile by running `dotnet build` and run the tests by using `dotnet test`. There are currently 20 integration tests in the solution (See `Api.Tests` directory).
- You can start up a "real" instance of the application either by pressing `F5` or by executing the `dotnet watch run` command from the `\Api` directory. With `watch`, as you make edits to the source, they'll be reflected automatically (no need to manually build every time!).
  - Once the app is running, you can see a [Swagger API definition in `localhost:5001`](https://localhost:5001/swagger/index.html).
- I use Postman for testing using a REST client. Here's the collection, you can import it and you'll find all relevant HTTP requests already configured.
  - [Download Postman](https://www.postman.com/downloads/)
  - [Get my exported collection](../../raw/main/Postman%20Collection%20Export.postman_collection.json)
  - [Import the collection to your Postman client](https://www.softwaretestinghelp.com/postman-collections-import-export-generate-code/)

## Assumptions, Suggestions, Improvements

- When adding friends, I'm ignoring invalid user ids. We store only friends that have an valid user in db.
- I'm not allowing neither amount of games played nor score to decrease.
- I'm assuming it is ok to use in-memory collections to simulate the database for now.
- Usernames are not unique, since we might have multiple players that would like to use the same name (e.g. MightyWarrior). Usernames are treated as display names and not account names. On the other hand, user UUIDs (GUIDs in .NET Core) are of course unique.
- Score is a natural number (0 <= Int64.MaxValue). We could use `BigInteger` for extremely long scores, but I kept it simple since it wasn't required.
- Game state (`gamesPlayed` and `score`) is created as part of the POST user workflow. The assumption here is that all players _should_ have a game state stored. On the other hand, we're not creating an empty friends list as part of this workflow. The rationale behind this is that there might be just a few users that use the friends feature, so it's probably better to not save empty friends documents for absolutely everyone.
- I would recommend to use pagination for the `Get All Users` debug endpoint.
- I think it makes sense to have `UpdateFriends` as an idempotent (PUT) operation. However, it might be worth passing only a list of new friends, and then have another endpoint to unfriend (DELETE). This way we don't have to send a potentially big payload every single time the user makes an update to their friends list.
- Proper authentication and authorization needs to be added (e.g. JWT tokens with claims).
- Related to the previous point, we shouldn't expose the Debug endpoint externally. We could have a `/debug/*` URL route that would require admin credentials to be accessed.
- Instead of `Console.WriteLine`, it would be better to use a logging service like log4net. This will allow for better log granularity (e.g., debug, info, error).
- As the game grows, metrics would be important to keep track of useful stats (e.g., ratio of successful requests, errors, CPU and memory usage).
- To be able to scale easily, a microservices architecture is recommended. Since the current app is decoupled, this could be easily achievable after containerizing independent modules. At the moment the app is quite small so it may not make sense yet.
- The `UserService` should be broken into other services if the amount of enpoints keeps growing.
- For HTTP requests, responses, and verbs guidelines, I follow the recommendations found in the [RFC 2616](https://www.w3.org/Protocols/rfc2616/rfc2616.html) specification.
- It's a general good practice to use (Github flow)[https://guides.github.com/introduction/flow/]. Since I'm the only contributor to this project (and I'm not working on multiple features on parallel), I've been using `main/master` directly. Otherwise, it's recommended to work on branches and submit a pull request to master once done.
- Tests have been incorporated progressively in a TDD fashion. Every time new functionality was being added, relevant tests were included in the commit. This is to prove that the new features are working as expected and therefore avoid pushing broken code that compiles but might have flawed logic.
- The AAA (Arrange, Act, Assert) pattern is a common way of writing tests. I wouldn't enforce it too much if the team finds it annoying.
- Used [Dependency Injection](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-3.1#services-injected-into-startup) when it makes sense.
