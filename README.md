# Heracles

`heracles` allows entries in the University of Oxford Hydra DNS to be queried, added, modified and deleted on the command line.

***As is common with UNIX command line tools, `heracles` does not ask for confirmation before performing bulk actions. `heracles search "." | heracles delete` will happily clear your records.*** If you do make a terrible mistake, note that the output text of most commands is either valid input for other commands, or can be used to create it.

# Compiling

`heracles` can be compilied to a native executable anywhere the DotNet 8 SDK is available.

1. Install the DotNet 8 SDK. See https://dotnet.microsoft.com/en-us/download for full details.
   - Windows: `winget install Microsoft.DotNet.SDK.8`
   - Ubuntu: `apt install dotnet-sdk-8.0`
   - Mac: `brew install dotnet-sdk`
2. Install the necessary compiler tools for your platform. See https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/#prerequisites for full details.
3. `dotnet publish`

This will create a native executable for the OS and architecture the code was compiled on, in the folder `Heracles.Console\bin\Release\net8.0\{os}-{arch}\publish\`. The `.pdb` debugging files can be ignored.

# Requirements

You must create an API token which can be used by Heracles, as described at https://wiki.it.ox.ac.uk/networks/HydraTokens. To enable the use of different tokens with different privileges, as well as using Heracles with both production and sandpit Hydra instances, the access credentials and the URI of the API must be stored in environment variables:

- `HYDRA_URI`: URI for either production or sandpit, as documented at https://wiki.it.ox.ac.uk/networks/HydraAPI, and ending `/ipam/` (including the trailing slash)
- `HYDRA_TOKEN` : auth token in the format `unit/user:encodedpassword` (including the `:`)

Environment variables were chosen because they can be easily populated by secrets managment systems and can be injected into containerised environments.

# Examples

## Example 1 - Searching for records matching a hostname string

Returns a list of JSON encoded records (if any).

```
$ heracles search _acme
[
  {
    "big_endian_labels": [
      "uk",
      "ac",
      "ox",
      "unit",
      "web",
      "_acme-challenge"
    ],
    ...
  }
]
```

## Example 2 - Getting matching records

Takes a JSON list containing one or more entries to be added, either on STDIN or as the second argument. `id`, `content`, `hostname` and `type` are required fields. Returns a JSON list of the entries. Primarily useful for safely testing command pipes.

```
$ heracles search _acme-challenge.web | heracles get
[
  {
    "big_endian_labels": [
      "uk",
      "ac",
      "ox",
      "unit",
      "web",
      "_acme-challenge"
    ],
    ...
  }
]
```
## Example 3 - Adding new records

Takes a JSON list containing one or more entries to be added, either on STDIN or as the second argument. `content`, `hostname`, and `type` are required fields. Returns a JSON list of the affected entries.

```
$ echo '[{ "comment": "Test", "content": "foofoofoofoofoofoofoofoo", "hostname": "_acme-challenge.foo.unit.ox.ac.uk.", "type": "TXT" }]' | heracles add
[
  {
    "big_endian_labels": [
      "uk",
      "ac",
      "ox",
      "unit",
      "foo",
      "_acme-challenge"
    ],
    "bind_rdata": "\u0022foofoofoofoofoofoofoofoo\u0022",
    "comment": "Test",
    "content": "foofoofoofoofoofoofoofoo",
    ...
  }
]
```

## Example 4 - Updating a record

Takes a JSON list containing one or more entries to be modified, either on STDIN or as the second argument. `id`, `content`, `hostname`, `type` and any properties to modify are required fields. Returns a JSON list of the affected entries.

```
$ heracles search _acme-challenge.foo | sed 's/comment": "Test"/comment": "Another Test"/' | heracles update
[
  {
    "big_endian_labels": [
      "uk",
      "ac",
      "ox",
      "unit",
      "foo",
      "_acme-challenge"
    ],
    "bind_rdata": "\u0022foofoofoofoofoofoofoofoo\u0022",
    "comment": "Another Test",
    "content": "foofoofoofoofoofoofoofoo",
    ...
  }
]
```

## Example 5 - Deleting a record

Takes a JSON list containing one or more entries to be deleted, either on STDIN or as the second argument.  `id`, `content`, `hostname` and `type` are required fields. Returns a JSON list of the affected entries.

```
$ heracles search _acme-challenge.foo | heracles delete
[
  {
    "big_endian_labels": [
      "uk",
      "ac",
      "ox",
      "unit",
      "foo",
      "_acme-challenge"
    ],
    "bind_rdata": "\u0022foofoofoofoofoofoofoofoo\u0022",
    "comment": "Another Test",
    "content": "foofoofoofoofoofoofoofoo",
    ...
  }
]

$ heracles search _acme-challenge.foo
[]
```