# What is BitSkinsBot?

[![Codacy Badge](https://api.codacy.com/project/badge/Grade/84007143ffb54edc9660726f6271d858)](https://www.codacy.com/manual/Captious99/BitSkinsBot?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=dmitrydnl/BitSkinsBot&amp;utm_campaign=Badge_Grade)
[![build status](https://travis-ci.com/dmitrydnl/BitSkinsBot.svg?branch=master)](https://travis-ci.com/dmitrydnl/BitSkinsBot)

Bot can buy and sell items on [BitSkins](https://bitskins.com) automatically and make profit. It is build on .NET Core 2.1. Bot using [BitSkinsApi NuGet package](https://github.com/dmitrydnl/BitSkinsApi).

# How do I use BitSkinsBot?
Necessary to create ```account_data.json``` file in ```BitSkinsBot``` project, in properties this file you must set _Copy to Output Directory_ to _Copy always_. After that need add in ```account_data.json``` this:
```js
{
  "ApiKey": "Your BitSkins api key",
  "SecretCode": "Your BitSkins secret code"
}
```
After that Build and Start project.

## License
This project is licensed under the GNU General Public License v3.0 - see the [LICENSE](https://github.com/dmitrydnl/BitSkinsBot/blob/master/LICENSE) file for details.
