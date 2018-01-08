Styles are referenced from the production folders. Symlinks should created to reference the production files / folders.

Create the following symlinks in ruhoh/atlas/

/stylesheets/style.css
/img
/fnt

These should reference their respective counterparts in /Atlas.Online.Web/Atlas.Online.Web/Content/themes/base

Ruhoh expects media to be served from a /media folder. To circumvent this, create a '/media' symlink in the /atlas theme folder pointing to the existing /img symlink.