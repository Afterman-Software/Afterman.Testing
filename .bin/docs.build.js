const shell = require('shelljs');
const path = require('path');
const fs = require('fs');

const hasGit = shell.which('git') != null && shell.which('git') != undefined;

if (hasGit !== true) {
  console.warn('Could not find "git" on this machine.  Automated publish will not work.');
}
else {
  console.log('Updating the documentation branch with docs updates from master');
  //shell.exec('git checkout gh-pages');
  //shell.exec('git checkout master -- docs/');
  shell.exec('git status');
  //shell.exec('git add --all && git commit -m "Retrieved docs updates from Master"');
}

const bookPath = path.resolve(__dirname, '..', '_book');
const inGitbook = path.resolve(bookPath, 'gitbook');
const inIndexHtml = path.resolve(bookPath, 'index.html');
const inSearchIndex = path.resolve(bookPath, 'search_index.json');

const docsPath = path.resolve(__dirname, '..');
const outGitbook = path.resolve(docsPath, 'gitbook');
const outIndexHtml = path.resolve(docsPath, 'index.html');
const outSearchIndex = path.resolve(docsPath, 'search_index.json');

const inReadme = path.resolve(docsPath, 'docs', 'README.md');
const outReadme = path.resolve(docsPath, 'README.md');

console.log('Cleaning up output directory');
[outGitbook, outIndexHtml, outSearchIndex].forEach(dst => {
  if (fs.existsSync(dst) === true) {
    console.log(`Found ${dst} so removing...`);
    shell.rm('-rf', dst);
  }
});

[
  [inGitbook, outGitbook], 
  [inIndexHtml, outIndexHtml], 
  [inSearchIndex, outSearchIndex],
  [inReadme, outReadme]
].forEach(pair => {
  let src = pair[0];
  let dst = pair[1];

  console.log(`Copying ${src} to ${dst}`);
  shell.cp('-R', src, dst);
});

console.log('Cleaning up source directory');
shell.rm('-rf', bookPath);

console.log('Publishing to gh-pages');
if (hasGit === true) {
  shell.exec('git add --all && git commit -am "Auto-commit for documentation"');
  shell.exec('git push origin gh-pages');
}
