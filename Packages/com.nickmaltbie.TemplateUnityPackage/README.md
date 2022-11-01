# Template Unity Package

Nick Maltbie's Template Unity Package for easily creating and sharing unity
packages over git and NPM.

Template Unity Package is an open source project hosted at
[https://github.com/nicholas-maltbie/TemplateUnityPackage](https://github.com/nicholas-maltbie/TemplateUnityPackage)

This is an open source project licensed under a [MIT License](LICENSE.txt).

## Installation

Install the latest version of the project by importing a project via git
at this URL:
`https://github.com/nicholas-maltbie/TemplateUnityPackage.git#release/latest`

If you want to reference a specific tag of the project such as version `v1.0.0`,
add a `release/#v1.0.0` to the end of the git URL to download the package
from th auto-generated branch for that release. An example of importing `v1.0.0`
would look like this:

```text
https://github.com/nicholas-maltbie/TemplateUnityPackage.git#release/v0.1.0
```

To use the latest release, simply reference:

```text
https://github.com/nicholas-maltbie/TemplateUnityPackage.git#release/latest
```

For a full list of all tags, check the [TemplateUnityPackage Tags](https://github.com/nicholas-maltbie/TemplateUnityPackage/tags)
list on github. I will usually associated a tag with each release of the project.

_Note_: before I started using the package format for the project, I manually
released a unity package you needed to import. Any version before `v1.0.0`
will not work to import the project.

If you do not include a tag, this means that your project will update whenever
you reimport from main. This may cause some errors or problems due to
experimental or breaking changes in the project.

You can also import the project via a tarball if you download the source
code and extract it on your local machine. Make sure to import
via the package manifest defined at `Packages\com.nickmaltbie.TemplateUnityPackage\package.json`
within the project.

For more details about installing a project via git, see unity's documentation
on [Installing form a Git URL](https://docs.unity3d.com/Manual/upm-ui-giturl.html#:~:text=%20Select%20Add%20package%20from%20git%20URL%20from,repository%20directly%20rather%20than%20from%20a%20package%20registry.).

### Scoped Registry Install

If you wish to install the project via a
[Scoped Registry](https://docs.unity3d.com/Manual/upm-scoped.html)
and npm, you can add a scoped registry to your project from all of the
`com.nickmaltbie` packages like this:

```json
"scopedRegistries": [
  {
    "name": "nickmaltbie",
    "url": "https://registry.npmjs.org",
    "scopes": [
      "com.nickmaltbie"
    ]
  }
]
```

Then, if you want to reference a version of the project, you simply
need to include the dependency with a version string and the unity package
manager will be able to download it from the registry at
`https://registry.npmjs.org`

```json
"dependencies": {
  "com.nickmaltbie.TemplateUnityPackage": "1.0.0",
}
```

### Tests

If you wish to include the testing code for this project, make sure to add
the `com.nickmaltbie.TemplateUnityPackage` to the testables
of the project manifest.

```json
  "testables": [
    "com.nickmaltbie.TemplateUnityPackage"
  ]
```

## Demo

You can see a demo of the project running here:
[https://nickmaltbie.com/TemplateUnityPackage/](https://nickmaltbie.com/TemplateUnityPackage/).
The project hosted on the website is up to date with the most recent
version on the `main` branch of this github repo
and is automatically deployed with each update to the codebase.

## Samples

The samples in the project include:
* ExampleSample - Example sample for Template Unity Package.

## Documentation

Documentation on the project and scripting API is found at
[https://nickmaltbie.com/TemplateUnityPackage/docs/](https://nickmaltbie.com/TemplateUnityPackage/docs/)
for the latest version of the codebase.
