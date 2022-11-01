# Package Setup and Installation

Custom packages can be used to easily share code and assets between different
unity projects. However, creating a package in unity can sometimes take
a bit of work to get right. The instructions on how to create custom
packages is provided on Unity's documentation:
[Creating Custom Package](https://docs.unity3d.com/2022.1/Documentation/Manual/CustomPackages.html)

This requires quite a bit of work to setup and creating samples can be
a bit difficult, so I created a script to help automate the process.

The script `setup-package.sh` can be used to create a branch
for a given version of the package's source code.

## Usage

Example Usage:

```bash
setup-package.sh $package_path [$tag]
```

Creates a package of the unity files at the `$package_path` folder
and will include the files form `./Assets/Samples` in the export
under the path `Samples~` to follow unity convention.

Will also preserve any `git-lfs` links for files to avoid
duplicating assets in the repo.

Arguments:

* `$package_path` - Required, path to package folder of project,
      Should be something like `Packages/com.companyname.packagename`
* `[$tag]` - Optional, tag version to checkout before building
      package. If provided, will create a new branch with
      the name pattern `release/$tag`

## Samples

Normally, a package samples would be stored in a folder called `Samples~`, this
is because unity will ignore those files in the editor and allow the user
to import them as requested. However, the `~` suffix makes it so that these
samples are difficult to manage within the editor as they are hidden.

To get around this problem, the setup script will copy any samples stored
in the folder `.Assets/Samples` to the folder `Samples~` and preserve
the git history of those files to avoid duplication and increasing the size of
the repo significantly.

## Importing the Package

Once the target branch is created, the package can be shared and installed
using multiple methodologies:

* via a git repo
* via an npm repository
* as a tarball
* as the decompressed files

### Installing via Git Repo

Install the latest version of the project by importing a project via git
at this URL:
`https://github.com/nicholas-maltbie/TemplateUnityPackage.git#release/latest`

If you want to reference a specific tag of the project such as version `v1.0.0`,
add a `release/#v1.0.0` to the end of the git URL to download the package
from th auto-generated branch for that release. An example of importing `v1.0.0`
would look like this:

```text
https://github.com/nicholas-maltbie/TemplateUnityPackage.git#release/v1.0.0
```

To use the latest release, simply reference:

```text
https://github.com/nicholas-maltbie/TemplateUnityPackage.git#release/latest
```

For a full list of all tags, check the
[TemplateUnityPackage Tags](https://github.com/nicholas-maltbie/TemplateUnityPackage/tags)
list on github. I will usually associated a tag with each release of the project.

If you do not include a tag, this means that your project will update whenever
you reimport from main. This may cause some errors or problems due to
experimental or breaking changes in the project.

For more details about installing a project via git, see unity's documentation
on [Installing form a Git URL](https://docs.unity3d.com/Manual/upm-ui-giturl.html#:~:text=%20Select%20Add%20package%20from%20git%20URL%20from,repository%20directly%20rather%20than%20from%20a%20package%20registry.).

### Installing via Tarball

You can also import the project via a tarball if you download the source
code and extract it on your local machine. Make sure to import
via the package manifest defined at
`Packages\com.nickmaltbie.TemplateUnityPackage\package.json`
within the project.

If you don't with to use the tarball, you can also download the uncompressed
files and import those into the project just as easily.

### Scoped Registry Install

To install via scoped registry, push the release branch to an npm repo using the
`npm publish` command.

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

## Importing Tests

If you wish to include the testing code for this project, make sure to add
the `com.nickmaltbie.TemplateUnityPackage` to the testables
of the project manifest.

```json
  "testables": [
    "com.nickmaltbie.TemplateUnityPackage"
  ]
```
