# Documentation Website

The documentation website generates a website for your package
just like this one. The website is generated and configured using
[docfx](https://dotnet.github.io/docfx/) to create a static website with the
following contents:

1. A landing page based off the README.md file of your repo.
1. A manual generated from the files and assets specified under the path
    `Documentation\manual` in the repo such as this website.
1. A changelog based on the changelog file at in the packages folder.
1. Generated documentation from the XML comments and structure of the C# code.

This website can be generated using `Documentation\build.cmd` or
`Documentation/build.sh`. This script will first generate the metadata from
the XML comments in the code, then generate the website
from the configuration provided at `Documentation\docfx.json`.

You can configure this website however fits best for your project. If you
want to make a sharable package, I suggest including documentation
on how the samples work and how to use the APIs included in the package.

The github action `deploy.yml` will also create a build of the
website and upload it to the `gh-pages` branch of your project.
If you configure the github repo to upload the `gh-pages` branch
to a static website, then you can host the website as part of the repo.
