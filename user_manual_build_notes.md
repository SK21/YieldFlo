# User Manual — how it is built and deployed

## Files

| File | Role |
|------|------|
| `YieldFlo\Help\YieldFlo User Manual.html` | **Authoritative source** — edit this (and keep the .md in step) |
| `YieldFlo\Help\YieldFlo User Manual.md` | Parallel draft kept in sync with the HTML |
| `YieldFlo\Help\YieldFlo User Manual.pdf` | Generated from the HTML — never edited by hand |
| `YieldFloApp\Help\*` | Deployed copies — what the app's Help button actually opens |

## Updating the manual

1. Edit the **HTML** and make the same change in the **.md**.
2. Regenerate the **PDF** from the HTML with headless Edge:

   ```
   msedge --headless --disable-gpu --no-pdf-header-footer ^
     --print-to-pdf="F:\Documents\GitHub\YieldFlo\YieldFlo\Help\YieldFlo User Manual.pdf" ^
     "file:///F:/Documents/GitHub/YieldFlo/YieldFlo/Help/YieldFlo%20User%20Manual.html"
   ```

3. Deploy: building the app copies `Help\**` to `YieldFloApp\Help`
   (csproj `Content Include="Help\**\*"` with `PreserveNewest`), or copy the
   three files there by hand if not building.

## How the app opens it

`frmMenu.btnHelp_Click` opens the **PDF** from `Help\` next to the exe,
falling back to the HTML, then the MD (see `YieldFlo\Forms\frmMenu.cs`).
This mirrors the RateController pattern (Help PDFs shipped beside the exe);
RC additionally has per-form PDFs (`Help\<formName>.pdf`) opened by its
menu Help button — YieldFlo currently ships the single full manual only.
