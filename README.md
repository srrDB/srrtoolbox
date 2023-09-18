# srrtoolbox

This is WIP and will be a mix of tools and things that can be used with srr-files and srrdb.com.

## Usage

Usage: srrtoolbox [input file]

[input file] is an AirDC-compatible file list.
The program will export a list of detected releases.

## config.json
Basic

    {
      "Configuration": {
        "AirDCExport": {
          "Format": "StructuredJson"
        }
      }
    }

Valid format options are: `RawTxt, FlatJson, StructuredJson` (todo: FlatXml, SturcutredXml)
