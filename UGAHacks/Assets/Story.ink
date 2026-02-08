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
// PATIENT 1: WITCH KITTY (THE ADMIRER)
// ==========================================

=== Witch_Start ===
~ Spawn("witchKitty")
WitchKitty: "Doc, you have to help me! I'm... I'm too plain!"
WitchKitty: "I need to look like HER. The Calico."

// --- CHOICE SET 1 ---
WitchKitty: "She's so popular and colorful. I'm just... white. Like a ghost."
+ [White cats are magical.] 
    ~ Stress(-5)
    WitchKitty: "I guess... but Calicos are lucky!"
+ [She might have problems too.] 
    ~ Stress(-10)
    WitchKitty: "No way! She's perfect!"
+ [Stop comparing yourself.] 
    ~ Stress(20)
    WitchKitty: "I can't help it! I'm invisible!"

- // GATHER

// --- CHOICE SET 2 ---
WitchKitty: "I found a 'Fur-Color' potion recipe. Help me cast it!"
+ [Self-love is the best magic.] 
    ~ Stress(5)
    WitchKitty: "Boring! I want spots!"
+ [Let's brew it.] 
    ~ Stress(0)
    WitchKitty: "Yes! Stir the cauldron!"
+ [That magic is unstable.] 
    ~ Stress(15)
    WitchKitty: "I don't care! I'll risk it!"

- // GATHER

// --- GAME 1: RHYTHM (Casting the Spell) ---
// Note: We do NOT transform yet. She plays the game as Witch Kitty.
System: CASTING ILLUSION SPELL... SYNC THE MANA.
~ LoadMinigame("RhythmGameScene") 
-> Witch_Midpoint

=== Witch_Midpoint ===
// --- TRANSFORMATION HAPPENS HERE NOW ---
~ Transform("ideal") 

WitchKitty: "Did it work? Am I... perfect?"
WitchKitty: "But wait... my skin feels itchy. And tight."

// --- CHOICE SET 3 ---
WitchKitty: "Is this what she feels like? It's heavy."
+ [Dispel the illusion.] 
    ~ Stress(-10)
    ~ Transform("normal")
    WitchKitty: "Oh thank goodness. I can breathe."
+ [Fight the side effects.] 
    ~ Stress(20)
    WitchKitty: "Ouch! It stings!"
+ [Stabilize the mana.] 
    ~ Stress(0)
    WitchKitty: "I'm trying... help me!"

- // GATHER

// --- CHOICE SET 4 ---
WitchKitty: "Maybe being her isn't as easy as it looks."
+ [Be the best White Witch.] 
    ~ Stress(-5)
+ [She probably admires you too.] 
    ~ Stress(-10)
+ [Just dye your hair next time.] 
    ~ Stress(5)

- // GATHER

// --- GAME 2: CLICKER (Stabilizing/Dispelling) ---
System: MANA REJECTION DETECTED. STABILIZE THE AURA.
~ LoadMinigame("ClickerGameScene")
-> Witch_End

=== Witch_End ===
WitchKitty: "I think I'll stay white. It matches my hat better anyway."
~ EndPatient()
System: PATIENT DISCHARGED.
+ [Call Next Patient] -> Calico_Start

// ==========================================
// PATIENT 2: THE CALICO CAT (SOCIAL ANXIETY)
// ==========================================

=== Calico_Start ===
~ Spawn("hoodedGuy") 
// Note: We use the "hoodedGuy" slot in GameManager but load the Calico data
Calico: "..."
Calico: (She hides behind her tail, trembling.)

// --- CHOICE SET 1 ---
Calico: "Please don't look at me... everyone always stares at my spots."
+ [They are admiring you.] 
    ~ Stress(10)
    Calico: "That makes it worse! I hate the attention!"
+ [I'll look away.] 
    ~ Stress(-5)
    Calico: "Thank you... it's too much pressure."
+ [Take a deep breath.] 
    ~ Stress(-5)

- // GATHER

// --- CHOICE SET 2 ---
Calico: "I try to go to parties, but I feel like I'm dodging arrows."
+ [Just be yourself.] 
    ~ Stress(5)
    Calico: "I don't know who that is anymore."
+ [Visualize a shield.] 
    ~ Stress(-5)
+ [You need to practice dodging.] 
    ~ Stress(0)
    Calico: "Dodging... yes. I can do that."

- // GATHER

// --- GAME 1: FLAPPY BIRD (Dodging Attention) ---
System: ANXIETY SPIKE. DODGE THE AWKWARD STARES.
~ LoadMinigame("FlappyCatScene")
-> Calico_Midpoint

=== Calico_Midpoint ===
Calico: "I made it through... but I'm trapped behind these walls I built."
Calico: "I push everyone away so they can't judge me."

// --- CHOICE SET 3 ---
Calico: "Is it safe to come out now?"
+ [Only when you're ready.] 
    ~ Stress(-5)
+ [You have to break the walls.] 
    ~ Stress(0)
    Calico: "Break them? That sounds scary."
+ [Stay hidden forever.] 
    ~ Stress(20)
    Calico: "No! I'm lonely!"

- // GATHER

// --- CHOICE SET 4 ---
Calico: "Okay. I want to break out. I want to say hello."
+ [I'll help you swing the hammer.] 
    ~ Stress(-5)
+ [Smash those insecurities!] 
    ~ Stress(-10)
+ [Don't hurt yourself.] 
    ~ Stress(5)

- // GATHER

// --- GAME 2: BREAKOUT (Breaking Walls) ---
System: INITIATING BREAKTHROUGH PROTOCOL.
~ LoadMinigame("BreakoutScene")
-> Calico_End

=== Calico_End ===
Calico: "The walls are down. Hi. I'm Callie."
~ EndPatient()
System: SHIFT COMPLETE.
-> END