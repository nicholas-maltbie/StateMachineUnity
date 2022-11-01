# Template GitHub Actions

Example github workflows for the project are stored at `.github/workflows` and
include the following workflows:

* `build-verification.yml` - Verify that project can properly.
* `create-package.yml` - Creates a package for a github branch and npmjs repo.
* `deploy.yml` - Builds and deploys project to the `gh-pages` branch.
* `format.yml` - Linting with `markdownlint`, `dotnet format`, and `docfx`.
* `tests-validation.yml` - Runs test validation on the project.

For many of these actions to work for your own repo, you must first
configure the unity license for your project. See Game CI's
[Activation](https://game.ci/docs/github/activation) documentation for
further details on how to include the `secrets.UNITY_LICENSE` in your project.

Many of the actions also use the variables defined in
`.github\variables\projectconfig.env` to share some common configuration.
This is a simple configuration for managing the a few variables:

* `PROJECT_NAME` - name of the project for managing builds.
* `UNITY_VERSION` - version of unity to use for project building and
    testing, for example: "2021.3.3f1"
* `PACKAGE_PATH` - Path that the package assets can be found at, for
    example: `./Packages/com.nickmaltbie.TemplateUnityPackage`

## Build Verification

`build-verification.yml`

The build verification workflow is simply a verification that the project
can build as expected. The `ScriptBatch.TestBuild_WebGL` function from
the file `Assets\Editor\ScriptBatch.cs` is used to build the project.

The build verification workflow runs for every pull request. It will
create a WebGL build of the project using
[game-ci/unity-builder](https://github.com/marketplace/actions/unity-builder).

This requires the `secrets.UNITY_LICENSE` be setup for your repo
to work properly.

## Create Package

`create-package.yml`

The create package workflow uses the `setup-package.sh` script to create
a package for the project then push it to a github branch and npmjs repo.
See the [Package Setup](package_setup.md) for further details about the script.

This uses the [Setup Node](https://github.com/marketplace/actions/setup-node-js-environment)
github action to configure node to publish the package.
This requires the `secrets.NPM_TOKEN` be setup with the proper permissions
to create new releases and packages.

This action is run every time a new tag is pushed to the repo or whenever a
new release is created for the project.

## Deploy

`deploy.yml`

The deploy is triggered every time a new commit is made to
the main branch of the repo. This action does three main things when launched

1. Create a new release at the branch `release/latest` based on the most recent
    code update for the repo.
1. Build the project using [game-ci/unity-builder](https://github.com/marketplace/actions/unity-builder)
    for WebGl and push this build to the `gh-pages` branch of the repo.
1. Build the documentation for the website using `docfx` and push this
    documentation to the `gh-pages` branch along with the unity WebGL build.
    For further information on the documentation website, see the
    [Documentation Website](documentation_website.md) page.

This requires the `secrets.UNITY_LICENSE` be setup for your repo
to work properly.

## Format Linting

`format.yml`

Format linting is looking for errors in the style of the project.
There are three kinds of linting applied to the project
with this script.

* C# Code linting with [dotnet format](https://github.com/dotnet/format).

    This is configured via the the file `.editorconfig` in the repo

* Markdown linting with [markdownlint](https://github.com/DavidAnson/markdownlint)
    via the github action [Markdown Linting Action](https://github.com/marketplace/actions/markdown-linting-action)

    This is configured to use default settings but has an ignore list
    of files provided at `.markdownlintignore`

* C# Documentation linting with [docfx](https://dotnet.github.io/docfx/)

    This simply creates a build of the documentation website via `docfx` and
    uses the `docfx` configuration at `docfx.json` to confirm that the
    documentation in the project is setup properly and can be generated
    without warnings or errors from the code.

## Tests Validation

`tests-validation.yml`

This workflow runs the tests in `EditMode` and `PlayMode` for the unity
project and generates a code coverage report.
Uses [Unity - Test Runner](https://github.com/marketplace/actions/unity-test-runner)
github action to run the unity tests.

This requires the `secrets.UNITY_LICENSE` be setup for your repo
to work properly.

This will also add a comment to the PR where it is run with the
line coverage percentage for the current tests combined in the project.
