# Heracles

`heracles` allows entries in the University of Oxford Hydra DNS to be queried, modified and deleted on the command line.

***As is common with UNIX command line tools, `heracles` does not ask for confirmation before performing bulk actions. `heracles search "." | heracles delete` will happily clear your records.*** If you do make a terrible mistake, note that the output text of most commands is valid input for most other commands (once unique fields like `id` are all stripped out).

# Compiling

`heracles` is a C# program which will run anywhere the DotNet 8 SDK is available. See https://dotnet.microsoft.com/en-us/download for full details.

- Windows: `winget install Microsoft.DotNet.SDK.8`
- Ubuntu: `apt install dotnet-sdk-8.0`
- Mac: `brew install dotnet-sdk`

# Requirements

You must create an API token which can be used by Heracles, as described at https://wiki.it.ox.ac.uk/networks/HydraTokens. To enable the use of different tokens with different privileges, as well as using Heracles with both production and sandpit Hydra instances, the access credentials and the URI of the API must be stored in two environment variables:

- `HYDRA_URI`: URI for either production or sandpit, as documented at https://wiki.it.ox.ac.uk/networks/HydraAPI, and ending `/ipam/` (including the trailing slash)
- `HYDRA_TOKEN` : auth token in the format `unit/user:encodedpassword` (including the `:`)

# Examples

## Example 1 - Searching for records matching a hostname string (no hits)

```
$ heracles search _acme
[]
```

## Example 2 - Adding a new record

```
$ echo '[{ "comment": "Test", "content": "foofoofoofoofoofoofoofoo", "hostname": "_acme-challenge.foo.imm.ox.ac.uk.", "type": "TXT" }]' | heracles add
[
  {
    "big_endian_labels": [
      "uk",
      "ac",
      "ox",
      "imm",
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

## Example 3 - Updating a record

```
$ heracles search _acme-challenge.foo | sed 's/comment": "Test"/comment": "Another Test"/' | heracles update
[
  {
    "big_endian_labels": [
      "uk",
      "ac",
      "ox",
      "imm",
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

## Example 4 - Deleting a record
```
$ heracles search _acme-challenge.foo | heracles delete
[
  {
    "big_endian_labels": [
      "uk",
      "ac",
      "ox",
      "imm",
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