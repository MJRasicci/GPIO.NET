# GPIO.NET CLI Workflow

## 1) Export a GP file to editable mapped JSON

```bash
dotnet run --project Source/GPIO.NET.Tool -- input.gp score.json --format json
```

## 2) Edit `score.json`

Edit the mapped score JSON and then write it back out through the full roundtrip writer.

## 3) Full rewrite while preserving archive payload

```bash
dotnet run --project Source/GPIO.NET.Tool -- score.json output.gp \
  --from-json --source-gp input.gp --format json \
  --diagnostics-out write-diagnostics.json --diagnostics-json
```

Use this when you want a full unmap/serialize write, but keep non-`score.gpif` zip entries
from an existing `.gp` archive:

## 4) Full rewrite without source GP

Use this for non-GP-originated scores. The writer seeds a default empty archive payload
(`VERSION`, `meta.json`, preferences, stylesheets, score views) and replaces `Content/score.gpif`.

```bash
dotnet run --project Source/GPIO.NET.Tool -- score.json output.gp \
  --from-json --format json
```
