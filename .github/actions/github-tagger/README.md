# Commit tagger

Tags the commit with a given text

## Getting Started
To use, create a workflow (eg: `.github/workflows/label.yml` see [Creating a Workflow file](https://help.github.com/en/articles/configuring-a-workflow#creating-a-workflow-file)) and add a step like 'Tag commit' on the below sample. A token will be needed so the workflow can make calls to GitHub's rest API.

```yaml
name: "My workflow"

on: [push]

jobs:
  build:
    runs-on: ubuntu-latest
    
    steps:
    - name: Tag commit
      uses: tvdias/github-tagger@v0.0.1
      with:
        repo-token: "${{ secrets.GITHUB_TOKEN }}"
        tag: "my_tag"
```

An optional commit sha can be specified to override the default

```yaml
    steps:
    - name: Tag commit
      uses: tvdias/github-tagger@v0.0.1
      with:
        repo-token: "${{ secrets.GITHUB_TOKEN }}"
        tag: "my_tag"
        commit-sha: abc123
```

If the tag already exists, the action will fail. If you wish to delete the existing tag and recreate it, add `move-existing: true`.

```yaml
    steps:
    - name: Tag commit
      uses: tvdias/github-tagger@v0.0.1
      with:
        repo-token: "${{ secrets.GITHUB_TOKEN }}"
        tag: "my_tag"
        move-existing: true
```
