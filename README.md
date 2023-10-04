# Docker

## Application

To start application execute next command:

```
docker-compose up -d api
```

Application start listening to <http://localhost:8080>.

## Tests

To runs test execute next command:

```
docker-compose run test
```

# About API

There is 3 endpoints:

## 1. ``POST /api/v1/:sheet_id/:cell_id``

with body

```
{
    "value": "{:cell_value}"    
}
```

```sheet_id``` - no restrictions. <br />
```cell_id``` - any not empty string, which do not starts with a digit, and can not contains next symbols ```' '```, ```'+'```,```'-'```,```'/'```,```'*'```,```'='```,```'('```,```')'```, ```','```, ```'.'```. <br />
```cell_value``` - any string (empty too), or math expression wich start with ```'='```, available operations is ```+, -, *, /``` and ```( )```, decimal parart of number should be separated by ```.```
A cell can be dependent on another one, and the max deep of recursive dependencies is ~3600.

### Responses

201 - cell created (upserted)<br />

```
{
    "value": ":cell_value",
    "result": ":processed_value"
}
```

400 - invalid ```cell_id``` <br />
422 - invalid ```cell_value```<br />

```
{
    "value": ":cell_value",
    "result": "ERROR"
}
``````

## 2. ```GET  /api/v1/:sheet_id/:cell_id```

### Responses

200 - cell was found<br />

```
{
    "value": ":cell_value",
    "result": ":processed_value"
}
```

400 - invalid ```cell_id``` <br />
404 - cell not found

## 3. ```GET /api/v1/:sheet_id```

### Responses

200 - table was found<br />

```
{
    "cell_1": {
        "value": ":cell_value_1",
        "result": ":processed_value_1"
    },
    "cell_2": {
        "value": ":cell_value_2",
        "result": ":processed_value_2"
    },
    ...
}
```

404 - sheet not found
