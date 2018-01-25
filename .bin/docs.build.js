const shell = require('shelljs');
const path = require('path');
const fs = require('fs');

const bookPath = path.resolve(__dirname, '..', '_book');
const inGitbook = path.resolve(bookPath, 'gitbook');
const inIndexHtml = path.resolve(bookPath, 'index.html');
const inSearchIndex = path.resolve(bookPath, 'search_index.json');

const docsPath = path.resolve(__dirname, '..', 'docs');
const outGitbook = path.resolve(docsPath, 'gitbook');
const outIndexHtml = path.resolve(docsPath, 'index.html');
const outSearchIndex = path.resolve(docsPath, 'search_index.json');

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
  [inSearchIndex, outSearchIndex]
].forEach(pair => {
  let src = pair[0];
  let dst = pair[1];

  console.log(`Copying ${src} to ${dst}`);
  shell.cp('-R', src, dst);
});

console.log('Cleaning up source directory');
shell.rm('-rf', bookPath);

console.log('Publishing to gh-pages');
if (shell.which('git') === true) {
  var code = shell.exec('git commit -am "Auto-commit"').code;
}
else {
  console.log('Publish to gh-pages failed: git cli not found on this machine');
}