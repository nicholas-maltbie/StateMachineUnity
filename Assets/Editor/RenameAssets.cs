#pragma warning disable IDE0073 // The file header does not match the required text

// Copyright (C) 2022 Nicholas Maltbie
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
// associated documentation files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
// BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jads.Tools;
using UnityEditor;
using UnityEngine;

public struct ReplaceStringObject
{
    public string name;
    public string source;
    public string dest;
    public bool apply;
}

/// <summary>
/// Window to select how to rename assets.
/// </summary>
public class RenameAssetsWindow : EditorWindow
{
    private readonly IEnumerable<string> ignorePrefixFilter = new string[]
    {
        ".git" + Path.DirectorySeparatorChar,
        ".vs",
        "Logs",
        "UserSettings",
        "Builds",
        Path.Join("Documentation", "manual"),
    };

    private readonly IEnumerable<string> includeDirections = new string[]
    {
        ".config",
        ".github",
        ".vscode",
        "Assets",
        "Demo",
        "Documentation",
        "Packages"
    };

    private readonly IEnumerable<string> includeSuffixFilter = new string[]
    {
        ".asmdef",
        ".cmd",
        ".cs",
        ".csproj",
        ".env",
        ".json",
        ".txt",
        ".md",
        ".sh",
        ".yml",
    };

    private const string sourceCompanyName = "nickmaltbie";
    private const string sourceProjectName = "Template Unity Package";
    private const string sourceWebsiteName = "https://nickmaltbie.com";
    private const string sourceGitHubUsername = "nicholas-maltbie";

    private bool regenerateGUIDs = true;

    private ReplaceStringObject companyNameReplace = new ReplaceStringObject
    {
        name = "Company",
        source = sourceCompanyName,
        dest = sourceCompanyName,
        apply = true,
    };

    private ReplaceStringObject projectNameReplace = new ReplaceStringObject
    {
        name = "Project",
        source = sourceProjectName,
        dest = sourceProjectName,
        apply = true,
    };

    private ReplaceStringObject websiteNameReplace = new ReplaceStringObject
    {
        name = "Website",
        source = sourceWebsiteName,
        dest = sourceWebsiteName,
        apply = true,
    };

    private ReplaceStringObject gitHubUsernameReplace = new ReplaceStringObject
    {
        name = "GitHub Username",
        source = sourceGitHubUsername,
        dest = sourceGitHubUsername,
        apply = true,
    };

    [MenuItem("Tools/Rename Template Assets")]
    public static void ShowRenameAssets()
    {
        // This method is called when the user selects the menu item in the Editor
        EditorWindow wnd = GetWindow<RenameAssetsWindow>();
        wnd.titleContent = new GUIContent("Rename Template Assets");
    }

    public void OnGUI()
    {
        PromptField(ref companyNameReplace);
        PromptField(ref projectNameReplace);
        PromptField(ref websiteNameReplace);
        PromptField(ref gitHubUsernameReplace);
        regenerateGUIDs = EditorGUILayout.Toggle($"Regenerate Asset GUIDs: ", regenerateGUIDs);

        if (GUILayout.Button("Rename Assets"))
        {
            OnClickRenameAssets();
            GUIUtility.ExitGUI();
        }
    }

    public void PromptField(ref ReplaceStringObject obj)
    {
        EditorGUILayout.LabelField($"Current {obj.name} Name: {obj.source}");
        obj.dest = EditorGUILayout.TextField($"Rename {obj.name} to", obj.dest);
        obj.apply = EditorGUILayout.Toggle($"Replace text in files", obj.apply);

        EditorGUILayout.Space();
    }

    public static string NoSpaces(string str) => str.Replace(" ", "");
    public static string Lower(string str) => str.ToLower();
    public static string Identity(string str) => str;
    public static string LowerNoSpaces(string str) => Lower(NoSpaces(str));

    public static IEnumerable<Func<string, string>> transformFunctions = new Func<string, string>[]
    {
        Identity,
        NoSpaces,
        Lower,
        LowerNoSpaces
    };

    public static string ProjectPath => Directory.GetParent(Application.dataPath).FullName;

    public static string PackagesPath => Path.Combine(ProjectPath, "Packages");

    public void OnClickRenameAssets()
    {
        (string, string)[] companyNameTransforms = GetTransformed(projectNameReplace.source, projectNameReplace.dest.Trim(), transformFunctions).ToArray();
        (string, string)[] projectNameTransforms = GetTransformed(companyNameReplace.source, companyNameReplace.dest.Trim(), transformFunctions).ToArray();
        (string, string)[] renameTargets = companyNameTransforms.Union(projectNameTransforms).ToArray();
        (string, string)[] replaceTargets = GetTransformed(gitHubUsernameReplace.source, gitHubUsernameReplace.dest.Trim(), transformFunctions)
            .Union(GetTransformed(websiteNameReplace.source, websiteNameReplace.dest.Trim(), transformFunctions))
            .Union(GetTransformed(companyNameReplace.source, companyNameReplace.dest.Trim(), transformFunctions))
            .Union(GetTransformed(projectNameReplace.source, projectNameReplace.dest.Trim(), transformFunctions))
            .Distinct()
            .ToArray();

        // Rename package path
        string packagePath = Directory.EnumerateDirectories(PackagesPath).FirstOrDefault(path =>
            companyNameTransforms.Any(pair => path.Contains(pair.Item1)) &&
            projectNameTransforms.Any(pair => path.Contains(pair.Item1)));

        if (packagePath == default)
        {
            EditorUtility.DisplayDialog(
                "Failed to Find Package",
                $"Could not find package folder in {ProjectPath} for\n" +
                $"Company Name: {sourceCompanyName}\nProject Name: {sourceProjectName}",
                "Ok");
            Close();
            return;
        }

        try
        {
            // regenerate guids for project files
            if (regenerateGUIDs)
            {
                EditorUtility.DisplayProgressBar($"Regenerating Guids", "Regenerating Guids, Step 1/4", 0);
                RegenerateGUIDS(new[]
                    {
                        Path.GetRelativePath(ProjectPath, packagePath),
                        Path.GetRelativePath(ProjectPath, Application.dataPath)
                    });
            }

            // Rename files that have project or company name in file name
            EditorUtility.DisplayProgressBar($"Renaming Files, Step 2/4", "...", 0);
            string[] filePaths = Directory.EnumerateFiles(packagePath, "*", SearchOption.AllDirectories).ToArray();
            int progress = 0;
            foreach (string path in filePaths)
            {
                progress++;
                EditorUtility.DisplayProgressBar(
                    $"Renaming Files, Step 2/4",
                    $"Checking File {Path.GetRelativePath(ProjectPath, path)}",
                    (float)progress / filePaths.Length);
                string relativePath = Path.GetRelativePath(ProjectPath, path);
                string targetPath = ApplyRenameTargets(relativePath, renameTargets);
                if (relativePath != targetPath)
                {
                    AssetDatabase.MoveAsset(relativePath, targetPath);
                }
            }

            // Replace contents of files that contain company name or project name
            EditorUtility.DisplayProgressBar($"Replacing Strings, Step 3/4", "...", 0);
            IEnumerable<string> allFilesInProject =
                Directory.EnumerateFiles(ProjectPath, "*", SearchOption.TopDirectoryOnly)
                .Union(includeDirections.SelectMany(dir =>
                    Directory.EnumerateFiles(Path.Join(ProjectPath, dir), "*", SearchOption.AllDirectories)));
            int totalFiles = allFilesInProject.Count();
            progress = 0;
            foreach (string path in allFilesInProject)
            {
                progress++;
                string relPath = Path.GetRelativePath(ProjectPath, path);
                string fileName = Path.GetFileName(path);
                bool skip = ignorePrefixFilter.Any(ignore => relPath.StartsWith(ignore)) ||
                    !includeSuffixFilter.Any(end => fileName.EndsWith(end));
                string op = skip ? "Skipping" : "Checking";
                EditorUtility.DisplayProgressBar(
                    $"Replacing Strings, Step 3/4",
                    $"{op} File {Path.GetRelativePath(ProjectPath, path)}",
                    (float)progress / totalFiles);

                if (skip)
                {
                    continue;
                }

                ReplaceTextInFiles(path, replaceTargets);
            }

            // Move package
            EditorUtility.DisplayProgressBar($"Moving Package Folder", $"{packagePath} to {renameTargets}", 1);
            packagePath = MovePackageFolder(packagePath, renameTargets);
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            Close();
        }
    }

    public static IEnumerable<(string, string)> GetTransformed(string source, string dest, IEnumerable<Func<string, string>> fns)
    {
        return fns.Select(fn => (fn(source), fn(dest))).Distinct();
    }

    public static void RegenerateGUIDS(string[] paths)
    {
        // Regenerate GUIDs for project folders
        IEnumerable<string> fileGUIDs = AssetDatabase.FindAssets("*", paths);

        AssetDatabase.StartAssetEditing();
        AssetGUIDRegenerator.RegenerateGUIDs(fileGUIDs.ToArray(), false);
        AssetDatabase.StopAssetEditing();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static string ApplyRenameTargets(string filePath, IEnumerable<(string, string)> renameTargets)
    {
        string targetPath = filePath;
        foreach ((string source, string dest) in renameTargets)
        {
            targetPath = Path.GetRelativePath(ProjectPath, GetRenamedLeaf(targetPath, source, dest));
        }

        return targetPath;
    }

    public static string MovePackageFolder(string packagePath, IEnumerable<(string, string)> renameTargets)
    {
        string targetPath = ApplyRenameTargets(packagePath, renameTargets);

        if (Path.GetRelativePath(ProjectPath, packagePath) == Path.GetRelativePath(ProjectPath, targetPath))
        {
            return targetPath;
        }

        Directory.Move(packagePath, targetPath);

        AssetDatabase.ImportAsset(targetPath, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ImportRecursive | ImportAssetOptions.ForceUpdate);
        AssetDatabase.Refresh();

        return targetPath;
    }

    public static string GetRenamedLeaf(string filePath, string source, string dest)
    {
        return Path.Combine(
            Directory.GetParent(filePath).FullName,
            Path.GetFileName(filePath).Replace(source, dest));
    }

    public static void ReplaceTextInFiles(string filePath, IEnumerable<(string, string)> replaceTargets)
    {
        string text = File.ReadAllText(filePath);
        foreach ((string source, string dest) in replaceTargets)
        {
            text = text.Replace(source, dest);
        }

        File.WriteAllText(filePath, text);
    }
}
