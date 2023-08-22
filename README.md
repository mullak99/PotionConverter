<h1 align="center">
  PotionConverter
</h1>
<p align="center">
    <a href="https://github.com/mullak99/Panorama/tree/master" alt="PotionConverter">
        <img src="https://github.com/mullak99/PotionConverter/actions/workflows/dotnet-build.yml/badge.svg" />
    </a>
    <a href="https://github.com/mullak99/PotionConverter/issues" alt="PotionConverter Issues">
        <img src="https://img.shields.io/github/issues/mullak99/PotionConverter" />
    </a>
    <a href="https://github.com/mullak99/PotionConverter/pulls" alt="PotionConverter Pull Requests">
        <img src="https://img.shields.io/github/issues-pr/mullak99/PotionConverter" />
    </a>
    <a href="https://github.com/mullak99/PotionConverter/stargazers" alt="PotionConverter Stars">
        <img src="https://img.shields.io/github/stars/mullak99/PotionConverter" />
    </a>
</p>

A standalone tool for converting Java Edition potion and tipped arrow textures to Bedrock.

Will potentially be integrated into [MCTools](https://github.com/mullak99/MCTools) and/or [Obsidian API](https://github.com/mullak99s-Faithful/Obsidian) at some point.

## Converting Java to Bedrock
1) [Download](https://github.com/mullak99/PotionConverter/actions/workflows/dotnet-build.yml) (Open the latest successful action, and download the artifact for your OS) and extract the tool somewhere
2) Drag the required files from a Java resourcepack into the same folder as the executable (listed below)
3) Run the PotionConverter executable
4) Converted files will be placed inside an "output" folder

### Required Files
- potion.png
- splash_potion.png
- lingering_potion.png
- potion_overlay.png
- tipped_arrow_base.png
- tipped_arrow_head.png

## Reverse Engineering Bedrock Colours
1) Do steps 1-2 as above
2) Drag the required files from a Java resourcepack into the same folder as the executable (listed below)
3) Add Bedrock potion textures to a "reverse" folder
3) Run PotionConverter with the "reverse" argument (e.g. "./PotionConverter reverse")
4) Colours will be outputted into "potions_reverseengineered.txt"

### Required Files
- potion.png
- splash_potion.png
- lingering_potion.png
- potion_overlay.png

## Notes
- The colours may be slightly different from Java. This seems to be due to slight differences with the floating-point rounding.
