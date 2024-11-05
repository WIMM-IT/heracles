# Heracles

`heracles` allows entries in the University of Oxford Hydra DNS to be queried and modified on the command line.

# Compiling

`heracles` is a C# program which will run anywhere the DotNet 8 SDK is available. See https://dotnet.microsoft.com/en-us/download for full details.

- Windows: `winget install Microsoft.DotNet.SDK.8`
- Ubuntu: `apt install dotnet-sdk-8.0`
- Mac: `brew install dotnet-sdk`

# Requirements

You must create an API token which can be used by Heracles, as described at https://wiki.it.ox.ac.uk/networks/HydraTokens. To enable the use of different tokens with different privileges, as well as using Heracles with both production and sandpit Hydra instances, the access credentials and the URI of the API must be stored in two environment variables:

- `HYDRA_URI`: URI for either production or sandpit, as documented at https://wiki.it.ox.ac.uk/networks/HydraAPI, and ending `/ipam/` (including the trailing slash)
- `HYDRA_TOKEN` : auth token in the format `unit/user:encodedpassword` (including the `:`)