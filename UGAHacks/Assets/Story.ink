EXTERNAL Spawn(name)
EXTERNAL Transform(form)
EXTERNAL LoadMinigame(sceneName)
EXTERNAL Stress(amount)
EXTERNAL EndPatient()

-> Start

=== Start ===
Another long night at the Arcane Ward...
+ [Ring the bell] -> Witch_Start

// ==========================================
// PATIENT 1: WITCH KITTY (THE ADMIRER)
// ==========================================

=== Witch_Start ===
~ Spawn("witchKitty")
WitchKitty: "Doc, you have to help me! I'm... I'm too plain!"
WitchKitty: "I need to look like HER. The Calico."
WitchKitty: "She's so popular and colorful. I'm just... white. Like a ghost."
+ [White cats are magical.]
    ~ Stress(5)
    WitchKitty: "I guess... but Calicos are lucky!"
+ [She might have problems too.]
    ~ Stress(-5)
    WitchKitty: "No way! She's perfect!"
+ [Stop comparing yourself.]
    ~ Stress(15)
    WitchKitty: "I can't help it! I'm invisible!"

- WitchKitty: "I found a 'Fur-Color' potion recipe. Help me cast it!"
+ [Self-love is the best magic.]
    ~ Stress(10)
    WitchKitty: "Boring! I want spots! You're not listening!"
+ [Let's brew it.]
    ~ Stress(5)
    WitchKitty: "Yes! Stir the cauldron!"
+ [That magic is unstable.]
    ~ Stress(15)
    WitchKitty: "I don't care! I'll risk it!"

- She's really going through with this... I need to help her channel the mana.
~ LoadMinigame("RhythmGameScene")
-> Witch_Midpoint

=== Witch_Midpoint ===
~ Transform("ideal")
WitchKitty: "Did it work? Am I... perfect?"
WitchKitty: "But wait... my skin feels itchy. And tight."
WitchKitty: "Is this what she feels like? It's heavy."
+ [Dispel the illusion.]
    ~ Stress(-5)
    ~ Transform("normal")
    WitchKitty: "Oh thank goodness. I can breathe."
+ [Fight the side effects.]
    ~ Stress(20)
    WitchKitty: "Ouch! It stings! Why did you tell me to push through?!"
+ [Stabilize the mana.]
    ~ Stress(10)
    WitchKitty: "I'm trying... it's barely holding together!"

- WitchKitty: "Maybe being her isn't as easy as it looks."
+ [Be the best White Witch.]
    ~ Stress(0)
    WitchKitty: "Maybe you're right... white IS magical."
+ [She probably admires you too.]
    ~ Stress(-5)
    WitchKitty: "You think so? That's... actually nice."
+ [Just dye your hair next time.]
    ~ Stress(10)
    WitchKitty: "Hmph! This is serious, Doc! You think this is a joke?!"

- Her aura is rejecting the spell... I need to stabilize it before it tears her apart.
~ LoadMinigame("ClickerGameScene")
-> Witch_End

=== Witch_End ===
WitchKitty: "I think I'll stay white. It matches my hat better anyway."
~ EndPatient()
She seems lighter now. One down.
+ [Call the next patient] -> Calico_Start

// ==========================================
// PATIENT 2: THE CALICO CAT (SOCIAL ANXIETY)
// ==========================================

=== Calico_Start ===
~ Spawn("calicoKitty")
Calico: "..."
Calico: (She hides behind her tail, trembling.)
Calico: "Please don't look at me... everyone always stares at my spots."
+ [They are admiring you.]
    ~ Stress(15)
    Calico: "That makes it worse! I hate the attention!"
+ [I'll look away.]
    ~ Stress(0)
    Calico: "Thank you... it's too much pressure."
+ [Take a deep breath.]
    ~ Stress(-5)
    Calico: "Okay... in... and out..."

- Calico: "I try to go to parties, but I feel like I'm dodging arrows."
+ [Just be yourself.]
    ~ Stress(10)
    Calico: "I don't know who that is anymore. Easy for you to say."
+ [Visualize a shield.]
    ~ Stress(-5)
    Calico: "A shield... okay, I'll try."
+ [You need to practice dodging.]
    ~ Stress(5)
    Calico: "Dodging... yes. I can do that."

- Her anxiety is spiking... I can feel it. She needs to learn to navigate through this.
~ LoadMinigame("FlappyCatScene")
-> Calico_Midpoint

=== Calico_Midpoint ===
Calico: "I made it through... but I'm trapped behind these walls I built."
Calico: "I push everyone away so they can't judge me."
Calico: "Is it safe to come out now?"
+ [Only when you're ready.]
    ~ Stress(0)
    Calico: "Okay... I'll take my time."
+ [You have to break the walls.]
    ~ Stress(10)
    Calico: "Break them?! That sounds terrifying... don't rush me!"
+ [Stay hidden forever.]
    ~ Stress(20)
    Calico: "No! I'm lonely! That's cruel!"

- Calico: "Okay. I want to break out. I want to say hello."
+ [I'll help you swing the hammer.]
    ~ Stress(-5)
    Calico: "Together then. Let's do this."
+ [Smash those insecurities!]
    ~ Stress(5)
    Calico: "That's... intense. But okay!"
+ [Don't hurt yourself.]
    ~ Stress(10)
    Calico: "You think I can't handle it? I'll show you..."

- Those walls she built... they're thick. Time to help her break through.
~ LoadMinigame("BreakoutScene")
-> Calico_End

=== Calico_End ===
Calico: "The walls are down. Hi. I'm Callie."
~ EndPatient()
The ward is quiet now. Shift's over. Maybe I helped them... maybe they helped me.
-> END