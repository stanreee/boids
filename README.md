# boids
 My take on the artificial life/intelligence program.
 
 ![caf0a82a53e11c6b29e20cc34b2fbf09](https://user-images.githubusercontent.com/77902731/109407235-0fd51e80-794d-11eb-994c-01dcd47fa914.gif)
 
 The different coloured boids represent different 'clusters' of boids. Any boid that joins these clusters alter their direction vector to travel in the average heading of the entire cluster, as well as towards the center of mass of each boid's respective cluster. The maximum sizes of boid clusters can be adjusted via the user interface on the right.

# background
 Boids is an artificial life program developed by Craig Reynolds. The program consists of flying entities, 'boids', that follow three rules:
 * separation: steer to avoid crowding local flockmates
 * alignment: steer towards average heading of local flockmates
 * cohesion: steer to move towards the average position (center of mass) of local flockmates

 My very simple interpretation of the boid program was created using the Unity game engine in C#.

# acknowledgements
 * [Boid Wikipedia] https://en.wikipedia.org/wiki/Boids
