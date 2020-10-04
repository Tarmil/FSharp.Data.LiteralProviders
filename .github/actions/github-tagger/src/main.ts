import * as core from "@actions/core";
import * as github from "@actions/github";

async function run() {
  try {
    const token = core.getInput("repo-token", { required: true });
    const tag = core.getInput("tag", { required: true });
    const sha =
      core.getInput("commit-sha", { required: false }) || github.context.sha;
    const moveExisting =
      core.getInput("move-existing", { required: false }) == "true";

    const client = new github.GitHub(token);

    const args = {
      owner: github.context.repo.owner,
      repo: github.context.repo.repo,
      ref: `refs/tags/${tag}`
    };

    if (moveExisting) {
      if (
        await client
          .request(`GET /repos/:owner/:repo/git/:ref`, args)
          .catch(_ => false)
      ) {
        core.debug(`deleting existing tag ${tag}`);
        await client.request(`DELETE /repos/:owner/:repo/git/:ref`, args);
      } else {
        core.debug(`tag ${tag} dosen't exist yet`);
      }
    }

    core.debug(`tagging #${sha} with tag ${tag}`);
    await client.git.createRef({ sha: sha, ...args });
  } catch (error) {
    core.error(error);
    core.setFailed(error.message);
  }
}

run();
