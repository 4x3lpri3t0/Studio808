## API

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