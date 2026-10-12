using System.Collections.Generic;
using UnityEngine;

public enum PersonalityType
{
    Impatient, // Sabırsız
    Generous,  // Cömert
    Sleepy,    // Uykusuz
    Gourmet,   // Gurme
    Friendly   // Dost canlısı
}

public static class CustomerDialogues
{
    // Cümlelerin içindeki {0} yazan yere sipariş ettiği kahvenin adı gelecek!
    public static Dictionary<PersonalityType, string[]> GreetingDialogues = new Dictionary<PersonalityType, string[]>()
    {
        {
            PersonalityType.Impatient, new string[] {
                "I'm in a huge rush! Make me a {0} fast!",
                "Clock is ticking! Just give me a {0} quickly!",
                "I have a meeting in 5 minutes! I need a {0} NOW!",
                "No time to waste! One {0} to go, please!",
                "Hurry up! I need a {0} right away!"
            }
        },
        {
            PersonalityType.Sleepy, new string[] {
                "I can barely open my eyes... Give me a {0} please.",
                "Pulled an all-nighter... I need a strong {0} to survive.",
                "Too tired to talk... Just a {0}, please...",
                "Running on zero energy... Is that {0} ready?",
                "I'm half asleep... One {0} will save my life."
            }
        },
        {
            PersonalityType.Gourmet, new string[] {
                "Let's test your barista skills. Prepare a {0} for me.",
                "I hope your {0} has the perfect foam and temperature.",
                "I'm here for the finest coffee. One {0}, please.",
                "Show me what you got. Make me a high quality {0}.",
                "I have high expectations for this {0}."
            }
        },
        {
            PersonalityType.Generous, new string[] {
                "What a lovely day! I'd like a {0}, take your time.",
                "Hello! Keep the energy up! One {0} please.",
                "Treating myself today! Make me a nice {0}.",
                "Smells amazing in here! I'll have a {0}.",
                "No rush at all! Just a {0} whenever you're ready."
            }
        },
        {
            PersonalityType.Friendly, new string[] {
                "Hi there! Could I get a {0}, please?",
                "Hello! How are you today? I'd like a {0}.",
                "Nice weather for a cold or hot drink! One {0} please.",
                "Hey! Just grabbing my daily {0}.",
                "Good day! A {0} sounds wonderful right now."
            }
        }
    };

    // Beklerken Sabrı Azalınca Söyleyeceği İngilizce Sözler
    public static Dictionary<PersonalityType, string[]> WaitingDialogues = new Dictionary<PersonalityType, string[]>()
    {
        {
            PersonalityType.Impatient, new string[] {
                "Still waiting?! I'm gonna be late!",
                "Are you growing the coffee beans yourself?!",
                "Come on, how hard is it to make a coffee?!"
            }
        },
        {
            PersonalityType.Sleepy, new string[] {
                "Falling asleep standing up...",
                "Is it ready yet... Zzz...",
                "My battery is at 1%... Please hurry..."
            }
        },
        {
            PersonalityType.Gourmet, new string[] {
                "Taking this long ruins the aroma...",
                "Slow service really lowers the quality.",
                "I expected better efficiency from a barista."
            }
        },
        {
            PersonalityType.Generous, new string[] {
                "Take your time, no worries!",
                "Looks like you're busy, I can wait a bit.",
                "No problem at all, do your magic!"
            }
        },
        {
            PersonalityType.Friendly, new string[] {
                "Is my drink coming up soon?",
                "Everything okay over there?",
                "Still working on my order?"
            }
        }
    };

    // Kahveyi Alınca Teşekkür Sözleri
    public static Dictionary<PersonalityType, string[]> SuccessDialogues = new Dictionary<PersonalityType, string[]>()
    {
        {
            PersonalityType.Impatient, new string[] { "Finally! Keep the change!", "Just in time! Thanks!" }
        },
        {
            PersonalityType.Sleepy, new string[] { "You saved my life... Thank you!", "Ah, caffeine! I feel alive!" }
        },
        {
            PersonalityType.Gourmet, new string[] { "Exquisite flavor! Well done.", "Worth the wait! Great balance." }
        },
        {
            PersonalityType.Generous, new string[] { "Awesome service! Left a big tip for you!", "Thank you so much! Have a great day!" }
        },
        {
            PersonalityType.Friendly, new string[] { "Thanks a lot! Tastes great!", "Thank you! Have a good one!" }
        }
    };

    // Cümlenin içine siparişi ({0}) yerleştiren metod
    public static string GetRandomSpeech(PersonalityType type, Dictionary<PersonalityType, string[]> dialogueGroup, string orderName = "")
    {
        if (dialogueGroup.ContainsKey(type))
        {
            string[] lines = dialogueGroup[type];
            string selectedLine = lines[Random.Range(0, lines.Length)];
            
            // Eğer sipariş adı varsa {0} yerine siparişi yazar
            return string.Format(selectedLine, orderName);
        }
        return "...";
    }
}