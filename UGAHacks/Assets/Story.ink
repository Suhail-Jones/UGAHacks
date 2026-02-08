EXTERNAL Spawn(name)
EXTERNAL Transform(form)
EXTERNAL LoadMinigame(sceneName)
EXTERNAL Stress(amount)
EXTERNAL EndPatient()

-> Start

=== Start ===
System: ARCANE WARD - NIGHT SHIFT.
System: Reputation: STABLE.
+ [Ring the Bell] -> Witch_Start

// ==========================================
// PATIENT 1: WITCH KITTY
// ==========================================

=== Witch_Start ===
~ Spawn("witchKitty")
WitchKitty: "U-um... hello? Is this the clinic?"
WitchKitty: "I need a... a personality transplant."

// --- CHOICE SET 1 ---
WitchKitty: "Everyone loves Calico cats. I'm just a black cat. It's unlucky."
+ [Luck is a superstition.] 
    ~ Stress(-5)
    WitchKitty: "I guess... but people still cross the street."
+ [You look cool to me.] 
    ~ Stress(-10)
    WitchKitty: "Really? You like the hat?"
+ [Yeah, black cats are bad omens.] 
    ~ Stress(20)
    WitchKitty: "I KNEW IT! I'm cursed!"

- // <--- THIS DASH IS A 'GATHER'. IT CATCHES ALL CHOICES ABOVE.

// --- CHOICE SET 2 ---
WitchKitty: "I tried casting a glamour spell to look like her."
+ [Magic can't fix self-worth.] 
    ~ Stress(5)
    WitchKitty: "But it can fix my fur color!"
+ [Show me the spell.] 
    ~ Stress(0)
    WitchKitty: "Okay, stand back!"
+ [That is forbidden magic!] 
    ~ Stress(15)
    WitchKitty: "I don't care! I need to change!"

- // <--- GATHER

// --- TRANSFORMATION ---
~ Transform("ideal")
System: ILLUSION CASTING... SYNC THE MANA WAVES.
~ LoadMinigame("RhythmGameScene") 
-> Witch_Midpoint

=== Witch_Midpoint ===
WitchKitty: "Did it work? Am I... perfect?"
WitchKitty: "It feels tight. Like wearing a mask."

// --- CHOICE SET 3 ---
WitchKitty: "I can't maintain this. It's draining my mana."
+ [Drop the spell.] 
    ~ Stress(-10)
    ~ Transform("normal")
    WitchKitty: "Phew... that's better."
+ [Push through the pain.] 
    ~ Stress(20)
    WitchKitty: "Ow! It burns!"
+ [Adjust the mana flow.] 
    ~ Stress(0)
    WitchKitty: "I'm trying... but it's slipping."

- // <--- GATHER

// --- CHOICE SET 4 ---
WitchKitty: "Maybe I don't need to be Calico. Maybe I just need to be a better Witch."
+ [Focus on your craft.] 
    ~ Stress(-5)
+ [Black cats are classic witches.] 
    ~ Stress(-10)
+ [Just become a dog instead.] 
    ~ Stress(30)
    WitchKitty: "Gross! No!"

- // <--- GATHER

// --- MINIGAME 2 ---
System: MANA OVERFLOW DETECTED.
~ LoadMinigame("ClickerGameScene")
-> Witch_End

=== Witch_End ===
WitchKitty: "I think I'll keep the hat. It suits me."
~ EndPatient()
System: PATIENT DISCHARGED.
+ [Call Next Patient] -> Hood_Start

// ==========================================
// PATIENT 2: HOODED GUY
// ==========================================

=== Hood_Start ===
~ Spawn("calicoKitty")
HoodedGuy: "..."
HoodedGuy: (He stands in the corner, hiding in his robe.)

// --- CHOICE SET 1 ---
HoodedGuy: "Too... many... eyes..."
+ [It's just me here.] 
    ~ Stress(-5)
    HoodedGuy: "Just you? Okay..."
+ [Speak up!] 
    ~ Stress(15)
    HoodedGuy: "eep!"
+ [I cast 'Zone of Privacy'.] 
    ~ Stress(-10)
    HoodedGuy: "Oh... the silence is nice."

- // <--- GATHER

// --- CHOICE SET 2 ---
HoodedGuy: "I wanted to join the guild. But I froze."
+ [Take deep breaths.] 
    ~ Stress(-5)
+ [Just kick the door down.] 
    ~ Stress(10)
+ [Try a disguise potion.] 
    ~ Stress(0)
    HoodedGuy: "Maybe... a mask would help."

- // <--- GATHER

// --- MINIGAME 1 ---
System: ANXIETY SPIKE. DEFLECT THE NEGATIVE THOUGHTS.
~ LoadMinigame("PongGameScene")
-> Hood_Midpoint

=== Hood_Midpoint ===
HoodedGuy: "Okay... I blocked them out."
HoodedGuy: "But what if I trip? What if I say something dumb?"

// --- CHOICE SET 3 ---
HoodedGuy: "Maybe I should just stay home forever."
+ [Isolation feeds fear.] 
    ~ Stress(-5)
+ [Home is safe.] 
    ~ Stress(5)
    HoodedGuy: "It is... but it's lonely."
+ [I'll teleport you there.] 
    ~ Stress(0)
    HoodedGuy: "No wait! I want to try."

- // <--- GATHER

// --- CHOICE SET 4 ---
HoodedGuy: "I think I'm ready to try. Just a little step."
+ [I believe in you.] 
    ~ Stress(-10)
+ [Don't mess up.] 
    ~ Stress(20)
+ [Take this luck charm.] 
    ~ Stress(-5)

- // <--- GATHER

// --- MINIGAME 2 ---
System: SOCIAL NAVIGATOR INITIALIZED.
~ LoadMinigame("FlappyGameScene")
-> Hood_End

=== Hood_End ===
HoodedGuy: "I... I think I can do this."
~ EndPatient()
System: SHIFT COMPLETE.
-> END