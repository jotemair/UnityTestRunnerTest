using System.Collections;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Test
{
    public class TestSuite
    {
        private GameObject game = null;
        private GameManager gameManager = null;
        private Player player = null;

        [SetUp]
        public void SetUp()
        {
            // Load game prefab
            GameObject gamePrefab = Resources.Load<GameObject>("Prefabs/Game");
            Assert.NotNull(gamePrefab, "Game prefab not found");

            // Instantiate the game
            game = Object.Instantiate(gamePrefab);
            Assert.NotNull(game, "Failed to instantiate game");

            // Get GameManager
            gameManager = GameManager.Instance;

            // Get player
            player = game.GetComponentInChildren<Player>();

            // Alternatively, but this is more resource intensive
            // player = Object.FindObjectOfType<Player>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(game);
        }

        // Test to check if GameObject with Player component is present
        [UnityTest]
        public IEnumerator PlayerExists()
        {
            // Wait until the end of frame
            yield return new WaitForEndOfFrame();

            // Check for player
            Assert.NotNull(player, "Player not child of game prefab");
        }

        // Test to see if GameManager singleton is available
        [UnityTest]
        public IEnumerator GameManagerExists()
        {
            // Wait until the end of frame
            yield return new WaitForEndOfFrame();

            // Check for GameManager
            Assert.NotNull(gameManager, "GameManager missing");
        }

        // Check if player can collide with items and pick them up
        [UnityTest]
        public IEnumerator ItemPlayerCollision()
        {
            // Load Item prefab
            GameObject itemPrefab = Resources.Load<GameObject>("Prefabs/Entities/Item");
            Assert.NotNull(itemPrefab, "Item prefab not found");

            // Check the score before item pickup
            int oldScore = gameManager.score;

            // Instantiate an Item instance at the players location
            GameObject item = Object.Instantiate<GameObject>(itemPrefab, player.transform.position, Quaternion.identity);
            Assert.NotNull(item, "Item not instantiated");

            // Wait for a fixed update, and then until the end of frame
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();

            // The item should have been collected by the player
            Assert.True(null == item, "Item not destroyed after collection");

            // This type of check fails, since Unity will still hang onto a "nulled" object
            // Assert.Null(item, "Item not destroyed after collection");

            // Check to see if the score was properly increased
            Assert.AreEqual(oldScore + 1, gameManager.score, "Score did not increase by one");
        }

        // Test to see if player is affected by gravity
        [UnityTest]
        public IEnumerator GravityAffectsPlayer()
        {
            // Wait until the end of frame to allow initial vatiables to be set
            yield return new WaitForEndOfFrame();

            // The player should start grounded
            Assert.True(player.isGrounded, "Player should start grounded for this test");

            // Move the player up
            player.transform.position += Vector3.up;

            // Wait a bit to allow variables to react to changes
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();

            // The player should no longer be grounded
            Assert.False(player.isGrounded, "Player should be in the air");

            // Wait until the player has time to fall to the ground
            yield return new WaitForSeconds(1f);
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();

            // The player should be grounded again
            Assert.True(player.isGrounded, "Player should start grounded");
        }

        // Test to see if the player can jump while on the ground
        [UnityTest]
        public IEnumerator PlayerCanJumpOnGround()
        {
            // Wait for initial variables to be set
            yield return new WaitForEndOfFrame();

            // The player should start grounded
            Assert.True(player.isGrounded, "Player should start grounded for this test");

            // Record the current player hight position, and perform a jump
            float startHeight = player.transform.position.y;
            player.Jump();

            // Wait just a bit to allow the jump to happen
            yield return new WaitForSeconds(.1f);
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();

            // Check if the player is higher
            Assert.Greater(player.transform.position.y, startHeight, "Player should move up when jumping");
        }

        // Test to make sure the player can't jump while in the air
        [UnityTest]
        public IEnumerator PlayerCantJumpInAir()
        {
            // Move the player in up in the air
            player.transform.position += Vector3.up;

            // Wait a bit for variables to set
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();

            // Record current Y position, and try to jump
            float startHeight = player.transform.position.y;
            player.Jump();

            // Allow a bit of time to pass
            yield return new WaitForSeconds(.1f);
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();

            // Check that the player kept falling
            Assert.LessOrEqual(player.transform.position.y, startHeight, "Player should not be able to jump in air");
        }

        // Test if the player can move sideways
        [UnityTest]
        public IEnumerator PlayerCanMove()
        {
            // Record the starting position of the player
            Vector3 startPosition = player.transform.position;

            // Wait a bit to check that the player does not move on it's own
            yield return new WaitForSeconds(.1f);
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();

            Assert.AreEqual(player.transform.position, startPosition, "Player should not move on its own");

            // Issue move command
            player.Move(1f, 1f);

            // Wait a bit to allow movement to occur
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();

            // Check that theplayer moved
            Assert.AreNotEqual(player.transform.position, startPosition, "Player should move on move input");
        }

        // Check that the player can detect targets in front of them
        [UnityTest]
        public IEnumerator PlayerCanShoot()
        {
            // Make sure that in the initial setup there aren't any valid targets
            Assert.False(player.Shoot(), "There should not be valid target in front on player at start of test");

            // Wait a frame
            yield return new WaitForEndOfFrame();

            // Instantiate an item, that is a valid target, in front of the player
            GameObject itemPrefab = Resources.Load<GameObject>("Prefabs/Entities/Item");
            Assert.NotNull(itemPrefab, "Item prefab not found");
            GameObject item = Object.Instantiate<GameObject>(itemPrefab, player.transform.position + Vector3.forward * 2, Quaternion.identity);

            // Wait a frame
            yield return new WaitForEndOfFrame();

            // Check that the player can detect the newly placed item
            Assert.True(player.Shoot(), "Player hits item");

            // Remove the item
            Object.Destroy(item);

            // Wait a frame for the item to disappear
            yield return new WaitForEndOfFrame();

            // Check that the player no longer has a valid target once more
            Assert.False(player.Shoot(), "There should no longer be a valid target");
        }
    }
}
