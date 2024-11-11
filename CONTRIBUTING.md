# Contributing to Heracles

There are several ways you can help with the development of `heracles`.

## Testing

`heracles` is still a young project, and the more extensively we can test the better. Currently, it's been used fairly extensively on Windows x64, MacOS Arm and Ubuntu Linux x64. In principle, it should be completley cross platform, but it would be good to have feedback on other successful deployments.

## Suggestions for improvement

We believe that the program is largely feature-complete, but we're always open to suggestions for additional features.

## Improving the certificate request scripts

The scrips for WinAcme and CertBot are trickier to test, especially in production. If you can find and fix bugs, or extend the scripts to enable additional functionality in either WinAcme or Certbot, we'd be very interested. We also be particularly interested in any contributions which enhanced error checking and/or improved the reliability of the scripts. Getting scripts in alternate languages, especially where those scripts can then be unit tested, would also be nice.

## Writing unit tests

If you're happy writing C#, it would be great to get better test coverage. Refactoring the code for Heracles.Lib or Heracles.Console to make it more unit-testable would also be useful.
