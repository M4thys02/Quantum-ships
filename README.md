# Quantum Ships  

**Quantum Ships** is a digital board game built in Unity, inspired by the classic *Battleships* but infused with the principles of quantum mechanics. Instead of hiding ships in static cells, players define a **probability distribution** of where their ship might be located.

The challenge lies in using measurements and logical reasoning to reconstruct your opponent's distribution before they decipher yours.

---

## Game Modes 
* **Local Multiplayer:** Two players compete on the same device in a turn-based format.
* **Single Player:** Work in progress - future planned.

## How It Works 

### Phase 1: The Layout
Each player is given a set of **Probability Squares** (representing a total probability of 1.0). You distribute these across an $N \times N$ grid, where $N$ ranges from 3 to 10.
* **Stacking:** Multiple squares can be stacked on a single cell.
* **Meaning:** More squares on a cell equals a higher probability that the "quantum ship" will be detected there during a measurement.
* **Auto-Layout:** If you prefer not to place squares manually, an automatic layout button is available.

### Phase 2: The Hunt
Players take turns performing actions to gather data or strike. You must perform at least one action before ending your turn.

1.  **Measure:** Perform a quantum measurement by clicking the Mysterious Bag. The result appears on the board (e.g., `B3 — 5`), meaning the ship was detected at that cell five times during that measurement burst.
2.  **Estimate & Attack:** Use "Estimation Squares" (red) to guess the opponent's exact layout. If your estimation for a cell is exactly correct, a green square appears to confirm the hit.

### The Physics of Strategy
The number of measurements ($M$) granted per turn is dynamically balanced based on the grid size ($N$) and the total squares ($P$) using the following formula:

$$M = 0.08 \cdot P + \frac{7}{N} + 1$$

---

## Installation & Play 

The game is available for **Windows**, **Linux**, and **Web Browser (WebGL)** (available in select releases).

1.  Navigate to the **Releases** section of this repository.
2.  **For Desktop:** Download the `.zip` for your OS, extract it, and run the executable directly.
3.  **For Web:** The WebGL build must be hosted on a web server (local or remote). It cannot be run by opening `index.html` directly due to browser security restrictions. For example, you can use a simple local server such as `python -m http.server`.

## Features 
* **Customizable Difficulty:** Adjust grid size ($3 \times 3$ to $10 \times 10$) and the total number of probability squares (from $10$ to $100$) in the settings.
* **Colorblind Mode:** Support for better visual accessibility.
* **Post-Game Analysis:** A "Reveal" screen compares your guesses side-by-side with the actual quantum state of the opponent's board to help you improve your strategy.

---

## Credits 
* **Original Board Game Author:** Mgr. Jana Legerská
* **Digital Version Developer:** Matěj Maroušek (Game Design, Programming, Graphics)
* **Scientific Consultant:** RNDr. Zdeňka Koupilová, Ph.D.
* **Assets:** UI elements based on *Fantasy Wooden GUI* (Unity Asset Store). Sound effects via Pixabay.

## License 
This project is licensed under the **MIT License**.

---
**Tip:** To win, remember that measurements are probabilistic—randomness applies, and high-probability cells are more likely, but not guaranteed, to be measured.
