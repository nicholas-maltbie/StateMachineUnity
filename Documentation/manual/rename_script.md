# Rename Script

Since this is a template repo, I wanted to make
renaming the project straightforward. Under the
menu option `Tools/Rename Template Assets` is a script
that will rename all the asset files in the project based
on a set of user settings as well as re-generate all
the GUIDs for unity assets to avoid asset collisions in the
future.

## Example Usage

The rename panel is allows for renaming assets in the project
from the template names to names for another project.

The panel currently supports four rename patterns:

1. Company Name - Applied to file names and text within files.
1. Project Name - Applied to file names and text within files.
1. Company Website - Applied to text within files only
1. Project GitHub Username - Applied to text within files only.

![
    View of rename panel in
    unity and various options.
](../resources/RenamePanel.png)

In the rename panel, each of the options has three
main attributes:

1. The current value for that field.
1. The value to replace with (default is no change)
1. Option to replace text in files. This is checked by
    default and will be replaced in all files.
    If this is not checked, it will not replace this
    text in existing files.

In addition, the final checkbox, `Regenerate Asset GUIDs`
will re-generate all asset GUIDs for the project to avoid
collisions with other projects. I recommend enabling this
option unless you need to keep the GUIDs the same
for some reason.

Once you are ready, hit the `Rename Assets` button
and the program will apply the renaming to all
the assets in the project.

If you are curious how the renaming works, look
at the `Assets\Editor\RenameAssets.cs` file in
the project for further details and source code.
