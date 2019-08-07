using System.Collections;
using System.Collections.Generic;
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

        [UnityTest]
        public IEnumerator PlayerExists()
        {
            yield return new WaitForEndOfFrame();

            Assert.NotNull(player, "Player not child of game prefab");
        }

        [UnityTest]
        public IEnumerator GameManagerExists()
        {
            yield return new WaitForEndOfFrame();

            Assert.NotNull(gameManager, "GameManager missing");
        }

        [UnityTest]
        public IEnumerator ItemPlayerCollision()
        {
            GameObject itemPrefab = Resources.Load<GameObject>("Prefabs/Entities/Item");
            Assert.NotNull(itemPrefab, "Item prefab not found");

            int oldScore = gameManager.score;

            GameObject item = Object.Instantiate<GameObject>(itemPrefab, player.transform.position, Quaternion.identity);
            Assert.NotNull(item, "Item not instantiated");

            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();

            Assert.True(null == item, "Item not destroyed after collection");

            // This fails, since Unity will still hang onto a "nulled" object
            // Assert.Null(item, "Item not destroyed after collection");

            Assert.AreEqual(oldScore + 1, gameManager.score, "Score did not increase by one");
        }

        [UnityTest]
        public IEnumerator GravityAffectsPlayer()
        {
            yield return new WaitForEndOfFrame();

            Assert.True(player.isGrounded, "Player should start grounded for this test");

            player.transform.position += Vector3.up;

            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();

            Assert.False(player.isGrounded, "Player should be in the air");

            yield return new WaitForSeconds(1f);
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();

            Assert.True(player.isGrounded, "Player should start grounded");
        }

        [UnityTest]
        public IEnumerator PlayerCanJumpOnGround()
        {
            yield return new WaitForEndOfFrame();

            Assert.True(player.isGrounded, "Player should start grounded for this test");

            float startHeight = player.transform.position.y;
            player.Jump();

            yield return new WaitForSeconds(.1f);
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();

            Assert.Greater(player.transform.position.y, startHeight, "Player should move up when jumping");
        }

        [UnityTest]
        public IEnumerator PlayerCantJumpInAir()
        {
            player.transform.position += Vector3.up;

            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();

            float startHeight = player.transform.position.y;
            player.Jump();

            yield return new WaitForSeconds(.1f);
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();

            Assert.LessOrEqual(player.transform.position.y, startHeight, "Player should not be able to jump in air");
        }

        [UnityTest]
        public IEnumerator PlayerCanMove()
        {
            Vector3 startPosition = player.transform.position;

            player.Move(1f, 1f);

            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();

            Assert.AreNotEqual(player.transform.position, startPosition, "Player should move on move input");
        }

        [UnityTest]
        public IEnumerator PlayerCanShoot()
        {
            Assert.False(player.Shoot(), "There should not be valid target in front on player at start of test");

            yield return new WaitForEndOfFrame();

            GameObject itemPrefab = Resources.Load<GameObject>("Prefabs/Entities/Item");
            Assert.NotNull(itemPrefab, "Item prefab not found");
            GameObject item = Object.Instantiate<GameObject>(itemPrefab, player.transform.position + Vector3.forward * 2, Quaternion.identity);

            yield return new WaitForEndOfFrame();

            Assert.True(player.Shoot(), "Player hits item");

            Object.Destroy(item);

            yield return new WaitForEndOfFrame();

            Assert.False(player.Shoot(), "There should no longer be a valid target");
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(game);
        }
    }
}
