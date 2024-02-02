using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Big_game_project_Testing
{
    public partial class Form1 : Form
    {
        Bitmap off;
        Hero hero;
        UI ui = new UI();
        List<Background> backgrounds = new List<Background>();
        List<Floor> floors = new List<Floor>();
        List<Floor> platforms = new List<Floor>();
        List<Boxes> boxes = new List<Boxes>();
        List<Item> items = new List<Item>();
        List<laserBox> laserBlocks = new List<laserBox>();
        List<bulletBox> bulletBoxes = new List<bulletBox>();
        List<Enemy> enemies = new List<Enemy>();
        List<Ladder> ladders = new List<Ladder>();
        List<Elevator> elevators = new List<Elevator>();
        List<Lever> levers = new List<Lever>();
        List<decortaiveLogs> logs = new List<decortaiveLogs>();
        List<Frog> frogs = new List<Frog>();
        List<Trap> traps = new List<Trap>();
        List<Gem> gems = new List<Gem>();
        List<Cherry> cherries = new List<Cherry>();
        Timer backgroundTimer = new Timer();
        Timer heroAnimation = new Timer();

        int previousLocation = 0, jumpCounter = 0, crate = -1, platformAnimation = 1, laserBoxInterval = 0, hurtFlag = 0, hurtTick = 0,
            elevatorTimer = 0, elevatorFinish = 0, timer = 0;
        string climbing = "none";
        public Form1()
        {
            InitializeComponent();
            this.Size = new Size(800, 700);
            this.CenterToScreen();
            this.Paint += Form1_Paint;
            this.Load += Form1_Load;
            this.KeyDown += Form1_KeyDown;
            this.KeyUp += Form1_KeyUp;
            backgroundTimer.Tick += Tt_Tick;
            backgroundTimer.Interval = 10;
            backgroundTimer.Start();
            heroAnimation.Tick += HeroAnimation_Tick;
            heroAnimation.Interval = 100;
            heroAnimation.Start();
        }
        
        void checkLaserDamage()
        {
            if (hero.laserFlag == 2)
            {
                for (int i = 0; i < enemies.Count; i++)
                {
                    if (enemies[i].x <= hero.x + 90 + 260 && enemies[i].y >= hero.y && enemies[i].y <= hero.y + 90)
                    {
                        enemies[i].liveState = 0;
                        enemies[i].state = 0;
                    }

                }
            }
        }
        bool checkDamage()
        {
            for (int i = 0; i < bulletBoxes[0].bullets.Count; i++)
            {
                if (bulletBoxes[0].bullets[i].x <= hero.x + 30 && bulletBoxes[0].bullets[i].x  >= hero.x
                    && bulletBoxes[0].bullets[i].y >= hero.y - 20 && bulletBoxes[0].bullets[i].y + 20 <= hero.y + 90)
                {
                    bulletBoxes[0].bullets.RemoveAt(i);
                    ui.hpState++;
                    return true;
                }
            }
            for (int i = 0; i < traps.Count; i++)
            {
                if (hero.x >= traps[i].x - 50 && hero.x <= traps[i].x + 30 + 10 &&
                    hero.y >= traps[i].y - 50 && hero.y <= traps[i].y + 30)
                {
                    ui.hpState++;
                    return true;
                }
            }
            for (int i = 0; i < frogs.Count; i++)
            {
                if (hero.x >= frogs[i].x - 50 && hero.x <= frogs[i].x + frogs[i].idle[0].Width + 10 &&
                    hero.y >= frogs[i].y - 50 && hero.y <= frogs[i].y + frogs[i].idle[0].Height && frogs[i].liveState == 1)
                {
                    ui.hpState++;
                    return true;
                }
            }
            for (int i = 0; i < enemies.Count; i++)
            {
                if (hero.x >= enemies[i].x - 90 && hero.x <= enemies[i].x + 110 &&
                    hero.y >= enemies[i].y - 50 && hero.y <= enemies[i].y + 50 && enemies[i].liveState == 1)
                {
                    ui.hpState++;
                    return true;
                }
            }
            for (int i = 0; i < laserBlocks.Count; i++)
            {
                if (hero.x + 90 >= laserBlocks[i].x - 250 && hero.x + 90 <= laserBlocks[i].x + 90 &&
                    hero.y >= laserBlocks[i].y - 20 && hero.y <= laserBlocks[i].y + 30 && laserBlocks[i].laserFlag == 1)
                {
                    ui.hpState++;
                    return true;
                }
            }
            return false;
        }
        bool checkObstacle()
        {

            for (int i = 0; i < boxes.Count; i++)
            {
                if ((hero.directionFlag == 0)
                    && hero.x + 90 >= boxes[i].x
                    && hero.x + 90 <= boxes[i].x + boxes[i].img[0].Width
                    && hero.y >= boxes[i].y - 40 && hero.y <= boxes[i].y + boxes[i].img[0].Height)
                    return false;
                if ((hero.directionFlag == 1)
                    && hero.x - 10 <= boxes[i].x + boxes[i].img[0].Width
                    && hero.x >= boxes[i].x
                    && hero.y >= boxes[i].y - 40 && hero.y <= boxes[i].y + boxes[i].img[0].Height)
                    return false;
            }
            return true;
        }
        void checkItems()
        {
            for (int i = 0; i < cherries.Count; i++)
            {
                if (hero.x >= cherries[i].x - 60
                    && hero.x <= cherries[i].x
                    && hero.y >= cherries[i].y - 50
                    && hero.y <= cherries[i].y
                    && cherries[i].taken == 0)
                {
                    ui.hpState = 0;
                    cherries[i].taken = 1;
                    cherries[i].state = 0;
                }
            }
            for (int i = 0; i < gems.Count; i++)
            {
                if (hero.x >= gems[i].x - 60 
                    && hero.x <= gems[i].x 
                    && hero.y >= gems[i].y - 30
                    && hero.y <= gems[i].y + 30 
                    && gems[i].taken == 0)
                {
                    gems[i].taken = 1;
                    gems[i].state = 0;
                }
            }
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].flag == 3) continue;
                if (hero.x <= items[i].x && hero.x + 80 >= items[i].x && hero.y <= items[i].y)
                    items[i].flag = 1;
            }
        }
        void changeItemState()
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].flag == 1)
                {
                    items[i].state++;
                    if (items[i].state >= 4)
                        items[i].flag = 3;
                }

            }
        }
        void boxesGravity()
        {
            int flag = 0;
            for (int i = 0; i < boxes.Count; i++)
            {
                flag = 0;
                for (int z = 0; z < boxes.Count; z++)
                {
                    if (boxes[i].y + boxes[i].img[0].Height + 20 >= boxes[z].y
                        && (boxes[i].x >= boxes[z].x - 30
                        && boxes[i].x <= boxes[z].x + boxes[z].img[0].Width))
                    {
                        if (z == i)
                            continue;
                        boxes[i].verticalOffset = 0;
                        flag = 1; break;
                    }
                }
                for (int z = 0; z < platforms.Count; z++)
                {
                    if (boxes[i].y + boxes[i].img[0].Height + 5 >= platforms[z].y
                        && boxes[i].y + boxes[i].img[0].Height + 5 <= platforms[z].y + platforms[z].img[0].Height
                        && boxes[i].x >= platforms[z].x - 20
                        && boxes[i].x + boxes[i].img[0].Width <= platforms[z].x + platforms[z].img[0].Width + 110)
                    {

                        boxes[i].verticalOffset = 0;
                        flag = 1; break;
                    }

                }
                for (int z = 0; z < floors.Count; z++)
                {
                    if (boxes[i].y + boxes[i].img[0].Height >= floors[z].y - 20)
                    {
                        boxes[i].verticalOffset = 0;
                        flag = 1; break;
                    }
                }
                if (flag == 0)
                {
                    boxes[i].verticalOffset = 3;

                }
            }
        }
        void animatePlatforms()
        {
            for (int i = 0; i < platforms.Count; i++)
            {
                if (platforms[i].animationFlag != 0)
                {
                    if (platforms[i].y >= 300)
                        platformAnimation = -1;
                    if (platforms[i].y <= 100)
                        platformAnimation = 1;
                    platforms[i].y += platformAnimation;
                }
            }
        }
        void laserStateChange()
        {
            if (hero.laserFlag == 1)
            {
                hero.laserState++;
                if (hero.laserState == 5)
                {
                    hero.laserFlag = 2;
                    hero.laserState = 0;
                }
            }
            if (hero.laserFlag == 2)
            {
                hero.laserState++;
                if (hero.laserState == 8)
                {
                    hero.laserFlag = 3;
                    hero.laserState = 0;
                }
            }
            if (hero.laserFlag == 3)
            {
                hero.laserState++;
                if (hero.laserState == 6)
                {
                    hero.laserFlag = 0;
                    hero.laserState = 0;
                }
            }
        }
        void changeBlockLaserState()
        {
            for (int i = 0; i < laserBlocks.Count; i++)
            {
                if (laserBlocks[i].laserFlag == 3)
                {
                    if (laserBoxInterval >= 30)
                    {
                        laserBlocks[i].laserFlag = 0;
                        laserBoxInterval = 0;
                    }
                    else
                        continue;
                }
                laserBlocks[i].state++;
                if (laserBlocks[i].laserFlag == 0)
                {
                    if (laserBlocks[i].state == 5)
                    {
                        laserBlocks[i].laserFlag = 1;
                        laserBlocks[i].state = 0;
                    }
                }
                else
                {
                    if (laserBlocks[i].laserFlag == 1)
                    {
                        if (laserBlocks[i].state == 8)
                        {
                            if (laserBoxInterval <= 20)
                            {
                                laserBlocks[i].state = 0;
                            }
                            else
                            {
                                laserBlocks[i].state = 0;
                                laserBlocks[i].laserFlag = 2;
                                laserBoxInterval = 0;
                            }
                        }

                    }
                    else
                    {
                        if (laserBlocks[i].laserFlag == 2)
                        {
                            if (laserBlocks[i].state == 5)
                            {
                                laserBlocks[i].laserFlag = 3;
                                laserBlocks[i].state = 0;
                            }
                        }
                    }
                }

            }
        }
        void changeEnemyState()
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].state++;
                if (enemies[i].type == 0)
                {
                    if (enemies[i].liveState == 1)
                    {
                        if (enemies[i].state == 4)
                            enemies[i].state = 0;
                    }
                    else
                    {
                        if (enemies[i].liveState == 0)
                            if (enemies[i].state == 6)
                                enemies.RemoveAt(i);
                    }
                }
                else if (enemies[i].type == 2)
                {
                    if (enemies[i].liveState == 1)
                    {
                        if (enemies[i].state == 6)
                            enemies[i].state = 0;
                    }
                    else
                    {
                        if (enemies[i].liveState == 0)
                            if (enemies[i].state == 6)
                                enemies.RemoveAt(i);
                    }
                }

            }
        }
        void changeFrog()
        {
            for (int i = 0; i < frogs.Count; i++)
            {
                if (timer % 25 == 0 && frogs[i].liveState == 1)
                {
                    frogs[i].jumpFlag = 1;
                    frogs[i].state = 0;
                }
                else
                {
                    if (frogs[i].jumpFlag != 1)
                    {
                        if (frogs[i].liveState == 0)
                        {
                            if (frogs[i].state == 3)
                            {
                                createGems(frogs[i].x, frogs[i].y);
                                frogs.RemoveAt(i);
                                continue;
                            }

                        }
                        frogs[i].state++;
                        if (frogs[i].state > 3)
                            frogs[i].state = 0;
                    }
                }
            }
        }
        void animateFrog()
        {
            for (int i = 0; i < frogs.Count; i++)
            {
                if (frogs[i].animationDirection == 0)
                {
                    if (frogs[i].jumpFlag == 1 && frogs[i].y >= frogs[i].rangeY && frogs[i].jumpFlag == 1 && frogs[i].state != 1
                        && frogs[i].animationDirection == 0)
                    {
                        frogs[i].y -= 6;
                        frogs[i].x -= 6;

                    }
                    else if (frogs[i].jumpFlag == 1 && frogs[i].y <= frogs[i].rangeY + 80)
                    {
                        frogs[i].y += 6;
                        frogs[i].x -= 6;
                        frogs[i].state = 1;
                    }
                    else
                    {
                        if (frogs[i].jumpFlag != 0)
                        {
                            frogs[i].animationDirection = 1;
                            frogs[i].jumpFlag = 0;
                        }
                    }
                }
                else
                {
                    if (frogs[i].jumpFlag == 1 && frogs[i].y >= frogs[i].rangeY && frogs[i].jumpFlag == 1 && frogs[i].state != 1
                        && frogs[i].animationDirection == 1)
                    {
                        frogs[i].y -= 6;
                        frogs[i].x += 6;

                    }
                    else if (frogs[i].jumpFlag == 1 && frogs[i].y <= frogs[i].rangeY + 80)
                    {
                        frogs[i].y += 6;
                        frogs[i].x += 6;
                        frogs[i].state = 1;
                    }
                    else
                    {
                        if (frogs[i].jumpFlag != 0)
                        {
                            frogs[i].animationDirection = 0;
                            frogs[i].jumpFlag = 0;
                        }
                    }
                }
            }
        }
        void changeGem()
        {
            for (int i = 0; i < gems.Count; i++)
            {
                gems[i].state++;
                if (gems[i].taken == 0)
                {
                    if (gems[i].state >= 5)
                        gems[i].state = 0;
                }
                else
                {
                    if (gems[i].state >= 4)
                    {
                        gems[i].state = 0;
                        gems.RemoveAt(i);
                    }
                }
            }
        }
        void changeCherry()
        {
            for (int i = 0; i < cherries.Count; i++)
            {
                cherries[i].state++;
                if (cherries[i].taken == 0)
                {
                    if (cherries[i].state >= 7)
                        cherries[i].state = 0;
                }
                else
                {
                    if (cherries[i].state >= 4)
                    {
                        cherries[i].state = 0;
                        cherries.RemoveAt(i);
                    }
                }
            }
        }
        void animateBoxBullet()
        {
            for(int i = 0; i < bulletBoxes[0].bullets.Count;i++)
            {
                if (bulletBoxes[0].bullets[i].interval <= 400)
                {
                    bulletBoxes[0].bullets[i].x -= 5;
                    bulletBoxes[0].bullets[i].interval += 5;
                }
                else
                    bulletBoxes[0].bullets.RemoveAt(i);
            }
        }
        void createBulletBoxBullet()
        {
            Bullet bullet1 = new Bullet();
            bullet1.x = bulletBoxes[0].x - 20;
            bullet1.y = bulletBoxes[0].y;
            bullet1.interval = 0;
            bulletBoxes[0].bullets.Add(bullet1);
        }
        private void HeroAnimation_Tick(object sender, EventArgs e)
        {
            timer++;
            if (timer % 1 == 0)
            {
                changeGem();
                changeCherry();
            }
            if (timer % 1 == 0)
            {
                changeFrog();

            }
            if (checkDamage() && hurtFlag != 1)
            {
                hero.hurtState = 1;
                hurtFlag = 1;
                hero.verticalOffset = -3;
                hero.horizontalOffset = -3;
            }
            else
            {
                if (hurtFlag == 1)
                {
                    hurtTick++;
                }
                if (hurtTick >= 3)
                {
                    hero.verticalOffset = 0;
                    hero.horizontalOffset = 0;
                    hero.idleState = -1;
                    hero.jumpFlag = 0;
                    hero.climbingState = -1;
                    hero.hurtState = -1;
                    hurtFlag = 0;
                    hurtTick = 0;
                }
            }
            if(hero.x >= bulletBoxes[0].x - 700 && timer%10 == 0)
            {
                createBulletBoxBullet();
            }
            changeItemState();
            changeBlockLaserState();
            changeEnemyState();
            laserStateChange();
            for (int i = 0; i < laserBlocks.Count; i++)
            {
                if (laserBlocks[i].laserFlag == 1)
                    laserBoxInterval++;
                if (laserBlocks[i].laserFlag == 3)
                    laserBoxInterval++;
            }
            if (hero.idleState != -1 && hero.laserFlag == 0)
            {
                hero.idleState++;
                if (hero.idleState > 3)
                    hero.idleState = 0;
            }
            if (hero.runningState != -1)
            {
                hero.runningState++;
                if (hero.runningState > 5)
                    hero.runningState = 0;
            }
            if (hero.climbingState != -1 && hero.verticalOffset != 0)
            {
                hero.climbingState++;
                if (hero.climbingState > 2)
                    hero.climbingState = 0;
            }
        }
        void moveBackground()
        {

            for (int i = 0; i < boxes.Count; i++)
                boxes[i].y += boxes[i].verticalOffset;
            if (checkObstacle())
            {
                backgrounds[0].src.X += backgrounds[0].offset;
                for (int i = 0; i < boxes.Count; i++)
                    boxes[i].x -= backgrounds[0].offset;
                for (int i = 0; i < platforms.Count; i++)
                    platforms[i].x -= backgrounds[0].offset;
                for (int i = 0; i < items.Count; i++)
                    items[i].x -= backgrounds[0].offset;
                for (int i = 0; i < floors.Count; i++)
                    floors[i].x -= backgrounds[0].offset;
                for (int i = 0; i < hero.bullets.Count; i++)
                    if (hero.bullets.Count != 0)
                        hero.bullets[i].x -= backgrounds[0].offset;
                for (int i = 0; i < laserBlocks.Count; i++)
                    laserBlocks[i].x -= backgrounds[0].offset;
                for (int i = 0; i < enemies.Count; i++)
                    enemies[i].x -= backgrounds[0].offset;
                for (int i = 0; i < ladders.Count; i++)
                    ladders[i].x -= backgrounds[0].offset;
                for (int i = 0; i < elevators.Count; i++)
                {
                    elevators[i].x -= backgrounds[0].offset;
                    if (hero.x < elevators[i].x - 600)
                        elevators[i].oldX -= backgrounds[0].offset;
                }
                for (int i = 0; i < levers.Count; i++)
                    levers[i].x -= backgrounds[0].offset;
                for (int i = 0; i < logs.Count; i++)
                    logs[i].x -= backgrounds[0].offset;
                for (int i = 0; i < frogs.Count; i++)
                    frogs[i].x -= backgrounds[0].offset;
                for (int i = 0; i < traps.Count; i++)
                    traps[i].x -= backgrounds[0].offset;
                for (int i = 0; i < gems.Count; i++)
                    gems[i].x -= backgrounds[0].offset;
                for (int i = 0; i < cherries.Count; i++)
                    cherries[i].x -= backgrounds[0].offset;
                for (int i = 0; i < bulletBoxes.Count; i++)
                {
                    bulletBoxes[i].x -= backgrounds[0].offset;
                    for (int z = 0; z < bulletBoxes[i].bullets.Count; z++)
                        bulletBoxes[i].bullets[i].x -= backgrounds[0].offset;
                }
               
            }
        }
        bool bulletCollision(int bullet)
        {

            for (int i = 0; i < frogs.Count; i++)
            {
                if (hero.bullets[bullet].x + 50 >= frogs[i].x && hero.bullets[bullet].x + 50 <= frogs[i].x + frogs[i].idle[0].Width + 10 &&
                    hero.bullets[bullet].y >= frogs[i].y - 50 && hero.bullets[bullet].y <= frogs[i].y && frogs[i].liveState == 1)
                {
                    frogs[i].liveState = 0;
                    frogs[i].state = 0;
                    createGems(frogs[i].x, frogs[i].y);
                    return false;
                }
            }
            for (int i = 0; i < enemies.Count; i++)
            {
                if (hero.bullets[bullet].x + 50 >= enemies[i].x && hero.bullets[bullet].x + 50 <= enemies[i].x + 110 &&
                    hero.bullets[bullet].y >= enemies[i].y - 50 && hero.bullets[bullet].y <= enemies[i].y && enemies[i].liveState == 1)
                {
                    enemies[i].liveState = 0;
                    enemies[i].state = 0;
                    createGems(enemies[i].x, enemies[i].y - 20);
                    return false;
                }
            }
            for (int i = 0; i < boxes.Count; i++)
            {
                if (hero.bullets[bullet].x + 50 >= boxes[i].x && hero.bullets[bullet].x + 50 <= boxes[i].x + 50 &&
                    hero.bullets[bullet].y >= boxes[i].y - 50 && hero.bullets[bullet].y <= boxes[i].y)
                    return false;
            }
            for (int i = 0; i < platforms.Count; i++)
            {
                if (hero.bullets[bullet].x + 50 >= platforms[i].x && hero.bullets[bullet].x + 50 <= platforms[i].x + 50 &&
                    hero.bullets[bullet].y >= platforms[i].y && hero.bullets[bullet].y <= platforms[i].y + 30)
                    return false;
            }
            return true;
        }
        void animateBullet()
        {
            for (int i = 0; i < hero.bullets.Count; i++)
            {
                if (bulletCollision(i))
                {
                    if (hero.bullets[i].directionFlag == 0)
                    {
                        if (hero.bullets[i].x > hero.x + 500)
                        {
                            hero.bullets.RemoveAt(i);
                            continue;
                        }

                        hero.bullets[i].x += 10;
                    }
                    else
                    {
                        if (hero.bullets[i].x < hero.x - 300)
                        {
                            hero.bullets.RemoveAt(i);
                            continue;
                        }
                        hero.bullets[i].x -= 10;
                    }
                }
                else
                {
                    hero.bullets.RemoveAt(i);
                }
            }
        }
        void heroJump()
        {
            if (hero.jumpFlag == 1)
            {
                hero.y -= hero.verticalOffset;
                if (hero.boxes.Count != 0)
                    hero.boxes[0].y -= hero.verticalOffset;
                if (hero.doubleJump == 0)
                {
                    if (previousLocation - hero.y > 140)
                    {
                        hero.jumpFlag = 0;
                    }
                }
                else
                {
                    if (hero.doubleJump == 1)
                    {
                        if (Math.Abs(previousLocation - hero.y) > 90)
                        {
                            hero.jumpFlag = 0;
                            hero.doubleJump = 2;
                        }
                    }
                }
            }
            else
            {
                hero.y += hero.verticalOffset;
                if (hero.boxes.Count != 0)
                    hero.boxes[0].y += hero.verticalOffset;
            }
        }
        void jump()
        {
            if (jumpCounter == 0)
            {
                hero.jumpingState = 0;
                hero.doubleJump = 0;
                hero.jumpFlag = 1;
                hero.verticalOffset = 15;
                if (previousLocation == 0)
                    previousLocation = hero.y;
            }
            else
            {
                if (hero.doubleJump == 0)
                {
                    hero.jumpingState = 0;
                    hero.jumpFlag = 1;
                    hero.verticalOffset = 15;
                    hero.doubleJump = 1;
                    previousLocation = hero.y;
                }
            }
        }
        void animateEnemies()
        {
            for (int i = 0; i < enemies.Count; i++)
            {

                if (enemies[i].liveState == 0)
                    continue;
                if (enemies[i].type == 0)
                {
                    if (enemies[i].y <= enemies[i].rangeY)
                        enemies[i].moveOffset = 1;
                    else
                    {
                        if (enemies[i].y >= enemies[i].rangeY + enemies[i].rangeOffset)
                            enemies[i].moveOffset = -1;
                    }
                    enemies[i].y += enemies[i].moveOffset;
                }
                else if (enemies[i].type == 2)
                {
                    if (enemies[i].trueX <= enemies[i].rangeX)
                    {
                        enemies[i].moveOffset = 1;
                        enemies[i].animationDirection = 1;
                    }
                    else
                    {
                        if (enemies[i].trueX >= enemies[i].rangeX + enemies[i].rangeOffset)
                        {
                            enemies[i].moveOffset = -1;
                            enemies[i].animationDirection = 0;
                        }
                    }
                    enemies[i].x += enemies[i].moveOffset;
                    enemies[i].trueX += enemies[i].moveOffset;
                }
            }
        }
        bool checkLadderCondition()
        {
            for (int i = 0; i < ladders.Count; i++)
            {
                if (climbing == "up")
                {
                    if (hero.y <= ladders[i].y - 90 && hero.x >= ladders[i].x - 30 && hero.x + 90 <= ladders[i].x + 90)
                        hero.verticalOffset = 0;
                }
                else
                {
                    if (hero.y + 90 >= ladders[i].y + 320 && hero.x >= ladders[i].x - 30 && hero.x + 90 <= ladders[i].x + 90)
                    {
                        hero.verticalOffset = 0;
                    }
                }
                if (hero.x >= ladders[i].x + 25 && hero.climb == 1)
                    backgrounds[0].offset = 0;
                if (hero.x + 90 <= ladders[i].x + 15 && hero.climb == 1)
                    backgrounds[0].offset = 0;
            }
            return true;
        }
        void animateElevator()
        {
            for (int i = 0; i < elevators.Count; i++)
            {
                if (levers[0].state == 1)
                {
                    if (elevators[i].horizontalOffset <= 220)
                    {
                        elevators[i].x -= 3;
                        elevators[i].horizontalOffset += 1;
                    }
                    else if (elevators[i].verticalOffset <= 110)
                    {
                        elevators[i].y -= 3;
                        elevators[i].verticalOffset += 1;
                    }
                    else
                        levers[0].state = 0;
                }
            }
        }
        void moveElevator()
        {
            for (int i = 0; i < elevators.Count; i++)
            {
                if (hero.x >= elevators[i].x - 30 && hero.x + 90 <= elevators[i].x + 170 &&
                    hero.y + 90 <= elevators[i].y + 120 && hero.y + 90 >= elevators[i].y + 110)
                {

                    elevatorTimer += 3;
                    if (elevatorTimer >= 90 && elevatorTimer <= 120)
                    {
                        elevators[i].moveState = 2;
                        elevators[i].horizontalOffset = 0;
                        elevators[i].verticalOffset = 0;
                    }
                    if (elevatorTimer >= 120 && elevators[i].moveState != 4)
                    {
                        if (elevatorTimer >= 125)
                            elevatorFinish = 1;
                        if (elevators[i].horizontalOffset <= 400)
                        {
                            elevators[i].x += 1;
                            elevators[i].horizontalOffset += 1;
                            backgrounds[0].offset = 1;
                        }
                        else if (elevators[i].verticalOffset <= 300)
                        {
                            if (elevators[i].moveState == 3)
                            {
                                elevators[i].y -= 1;
                                hero.verticalOffset = -1;
                            }
                            else
                            {
                                elevators[i].y += 1;
                                hero.verticalOffset = 1;
                            }
                            backgrounds[0].offset = 0;
                            elevators[i].verticalOffset += 1;
                        }
                        else
                        {
                            if (elevators[i].moveState == 2)
                            {
                                elevators[i].horizontalOffset = 0;
                                elevators[i].verticalOffset = 0;
                                elevators[i].moveState = 3;
                            }
                            else
                            {
                                elevators[i].moveState = 4;
                                elevatorFinish = 0;
                            }
                        }
                    }
                }
            }
        }
        void checkDeath()
        {
            if (ui.hpState / 5 >= 4)
            {
                ui.hpState = 3;
                heroAnimation.Stop();
                backgroundTimer.Stop();
            }
        }
        private void Tt_Tick(object sender, EventArgs e)
        {
            animateBoxBullet();
            checkDeath();
            animateFrog();
            animateElevator();
            moveElevator();
            checkLadderCondition();
            checkLaserDamage();
            animateEnemies();
            animateBullet();
            checkItems();
            boxesGravity();
            animatePlatforms();
            moveBackground();
            heroJump();
            gravity();
            if (backgrounds[0].src.X < 1)
            {
                backgrounds[0].offset = 0;
                backgrounds[0].src.X = 0;
            }
            drawDubb(this.CreateGraphics());
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (!checkLadder())
            {
                if ((e.KeyCode == Keys.D || e.KeyCode == Keys.A))
                {
                    backgrounds[0].offset = 0;
                }
            }
            else
            {
                if ((e.KeyCode == Keys.D || e.KeyCode == Keys.A) /*&& checkLadder()*/)
                {
                    hero.idleState = 0;
                    hero.runningState = -1;
                    hero.climbingState = -1;
                    hero.climb = 0;
                    backgrounds[0].offset = 0;
                }
            }
            if ((e.KeyCode == Keys.W || e.KeyCode == Keys.S) && !checkLadder())
            {
                hero.verticalOffset = 0;
                hero.climbingState = 0;

            }
            if (e.KeyCode == Keys.F)
            {
                //hero.laserFlag = 0;
                //laserX = 0;
            }

        }
        void moveCrate()
        {
            crate = findCrate();
            if (hero.boxFlag == 0)
            {
                if (crate != -1)
                {
                    hero.boxFlag = 1;
                    boxes[crate].moveFlag = 1;
                    hero.boxes.Add(boxes[crate]);
                    hero.boxes[0].y = hero.y - boxes[crate].img[0].Height + 10;
                    hero.boxes[0].x = hero.x + boxes[crate].img[0].Width + 10;
                    boxes.RemoveAt(crate);
                }
            }
            else
            {
                hero.boxes[0].y = hero.y - boxes[0].img[0].Height + 10;
                hero.boxes[0].x = hero.x + boxes[0].img[0].Width + 40;
                boxes.Add(hero.boxes[0]);
                boxes[boxes.Count - 1].moveFlag = 0;
                hero.boxFlag = 0;
                crate = -1;
                hero.boxes.Clear();
            }
        }
        bool checkLadder()
        {
            for (int i = 0; i < ladders.Count; i++)
            {
                if (hero.x >= ladders[i].x - 40 && hero.x + 90 <= ladders[i].x + 80 &&
                    hero.y >= ladders[i].y - 10 && hero.y + 90 <= ladders[i].y + 340)
                {
                    return false;
                }
            }
            return true;
        }
        void changeCrank()
        {
            for (int i = 0; i < levers.Count; i++)
            {
                if (hero.x >= levers[i].x - 40 && hero.x + 90 <= levers[i].x + 90)
                {
                    levers[i].state = 1;
                }
            }
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.D:
                    if (elevatorFinish == 0)
                    {
                        hero.directionFlag = 0;
                        backgrounds[0].offset = 3;
                        if (hero.idleState > -1)
                        {
                            hero.idleState = -1;
                            hero.climbingState = -1;
                            hero.runningState = 0;
                        }
                    }
                    break;
                case Keys.A:
                    if (elevatorFinish == 0)
                    {
                        if (backgrounds[0].src.X < 10)
                        {
                            backgrounds[0].offset = 0;
                            backgrounds[0].src.X = 0;
                        }
                        else
                        {
                            hero.directionFlag = 1;
                            backgrounds[0].offset = -3;
                            if (hero.idleState > -1)
                            {
                                hero.idleState = -1;
                                hero.climbingState = -1;
                                hero.runningState = 0;
                            }
                        }

                    }
                    break;
                case Keys.W:
                    if (!checkLadder())
                    {
                        climbing = "up";
                        hero.verticalOffset = -3;
                        backgrounds[0].offset = 0;
                        if (hero.climb == 0)
                            hero.climbingState = 0;
                        hero.climb = 1;
                        hero.idleState = -1;
                        hero.runningState = -1;
                        hero.jumpingState = -1;
                    }
                    break;
                case Keys.S:
                    if (!checkLadder())
                    {
                        climbing = "down";
                        hero.verticalOffset = 3;
                        backgrounds[0].offset = 0;
                        if (hero.climb == 0)
                            hero.climbingState = 0;
                        hero.climb = 1;
                        hero.idleState = -1;
                        hero.runningState = -1;
                        hero.jumpingState = -1;

                    }
                    break;
                case Keys.E:
                    changeCrank();
                    moveCrate();
                    break;
                case Keys.Space:
                    if (hero.climbingState == -1)
                        jump();
                    break;
                case Keys.F:
                    hero.laserFlag = 1;
                    hero.laserState = 1;
                    break;
                case Keys.Q:
                    if (ui.bulletState / 10 < 4)
                    {
                        createBullet();
                        ui.bulletState++;
                    }
                    break;
                case Keys.R:
                    ui.bulletState = 0;
                    break;
            }

        }
        void addBullet()
        {
            if (hero.directionFlag == 0)
            {
                Bullet bullet = new Bullet();
                bullet.x = hero.x + 30;
                bullet.y = hero.y + 5;
                bullet.directionFlag = hero.directionFlag;
                hero.bullets.Add(bullet);
            }
            else
            {
                Bullet bullet = new Bullet();
                bullet.x = hero.x - 30;
                bullet.y = hero.y + 5;
                bullet.directionFlag = hero.directionFlag;
                hero.bullets.Add(bullet);
            }
        }
        void createBullet()
        {
            if (hero.directionFlag == 0)
            {
                if (hero.bullets.Count > 1 && hero.bullets[hero.bullets.Count - 1].directionFlag == 0)
                {

                    if (hero.bullets[hero.bullets.Count - 1].x >= hero.x + 90)
                    {
                        addBullet();
                    }
                }
                else
                {
                    addBullet();
                }

            }
            else
            {
                if (hero.bullets.Count > 1 && hero.bullets[hero.bullets.Count - 1].directionFlag == 1)
                {
                    if (hero.bullets[hero.bullets.Count - 1].x <= hero.x - 90)
                    {
                        addBullet();
                    }
                }
                else
                    addBullet();
            }
        }
        int findCrate()
        {
            for (int i = 0; i < boxes.Count; i++)
                if (hero.y + 90 >= boxes[i].y
                && hero.y + 90 <= boxes[i].y + 120
                && hero.x >= boxes[i].x - 90
                && hero.x + 90 <= boxes[i].x + boxes[i].img[0].Width + 120)
                    return i;
            return -1;
        }
        void gravity()
        {
            int flag;
            if (hero.hurtState == -1 && hero.climbingState == -1 && checkLadder())
            {
                if (hero.jumpFlag == 0)
                {
                    //elevators
                    flag = 0;
                    for (int i = 0; i < elevators.Count; i++)
                    {
                        if (hero.x >= elevators[i].x - 30 && hero.x + 90 <= elevators[i].x + 170 &&
                            hero.y + 90 <= elevators[i].y + 120 && hero.y + 90 >= elevators[i].y + 110)
                        {
                            hero.verticalOffset = 0;
                            previousLocation = 0;
                            hero.jumpingState = -1;
                            jumpCounter = 0;
                            flag = 1;
                            break;
                        }
                    }
                    //enemies
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        if (hero.x >= enemies[i].x - 60 && hero.x <= enemies[i].x + 110 &&
                            hero.y + 90 >= enemies[i].y && hero.y + 90 <= enemies[i].y + 10 && enemies[i].liveState == 1)
                        {
                            enemies[i].liveState = 0;
                            enemies[i].state = 0;
                            hero.jumpFlag = 1;
                            hero.doubleJump = 0;
                            hero.verticalOffset = 0;
                            hero.jumpingState = 0;
                            jumpCounter = 0;
                            break;
                        }

                    }
                    for (int i = 0; i < frogs.Count; i++)
                    {
                        if (hero.x >= frogs[i].x - 40 && hero.x <= frogs[i].x + frogs[i].idle[0].Width + 10 &&
                            hero.y + 90 >= frogs[i].y && hero.y + 90 <= frogs[i].y + 10 && frogs[i].liveState == 1)
                        {
                            frogs[i].liveState = 0;
                            frogs[i].state = 0;
                            hero.jumpFlag = 1;
                            hero.doubleJump = 0;
                            hero.verticalOffset = 0;
                            hero.jumpingState = 0;
                            jumpCounter = 0;
                            break;
                        }

                    }
                    //ladders
                    for (int i = 0; i < ladders.Count; i++)
                    {
                        if (hero.x >= ladders[i].x - 30 && hero.x + 90 <= ladders[i].x + 100 &&
                            hero.y + 90 <= ladders[i].y)
                        {
                            hero.verticalOffset = 0;
                            previousLocation = 0;
                            hero.jumpingState = -1;
                            if (hero.runningState == -1)
                                hero.idleState = 0;
                            jumpCounter = 0;
                            flag = 1;
                            break;
                        }
                    }
                    //boxes
                    for (int i = 0; i < boxes.Count; i++)
                    {
                        if (hero.y + 90 >= boxes[i].y
                            && hero.y + 90 <= boxes[i].y + 40
                            && hero.x >= boxes[i].x - 60
                            && hero.x + 90 <= boxes[i].x + boxes[i].img[0].Width + 80)
                        {
                            hero.verticalOffset = 0;
                            previousLocation = 0;
                            if (hero.runningState == -1)
                                hero.idleState = 0;
                            hero.jumpingState = -1;
                            jumpCounter = 0;
                            flag = 1;
                            break;
                        }

                    }
                    //platforms
                    for (int i = 0; i < platforms.Count; i++)
                    {
                        if (platforms[i].platform == 3)
                        {
                            if (hero.y + 95 >= platforms[i].y + 10
                            && hero.y + 95 <= platforms[i].y + platforms[i].img[0].Height
                            && hero.x >= platforms[i].x - 40
                            && hero.x + 90 <= platforms[i].x + 440)
                            {
                                hero.verticalOffset = 0;
                                previousLocation = 0;
                                if (hero.runningState == -1)
                                    hero.idleState = 0;
                                hero.jumpingState = -1;
                                jumpCounter = 0;
                                flag = 1;
                                break;
                            }
                        }
                        if (platforms[i].platform == 2)
                        {
                            if (hero.y + 95 >= platforms[i].y + 10
                            && hero.y + 95 <= platforms[i].y + platforms[i].img[0].Height
                            && hero.x >= platforms[i].x - 40
                            && hero.x + 90 <= platforms[i].x + 220)
                            {
                                hero.verticalOffset = 0;
                                previousLocation = 0;
                                if (hero.runningState == -1)
                                    hero.idleState = 0;
                                hero.jumpingState = -1;
                                jumpCounter = 0;
                                flag = 1;
                                break;
                            }

                        }
                        if (hero.y + 95 >= platforms[i].y + 10
                            && hero.y + 95 <= platforms[i].y + platforms[i].img[0].Height
                            && hero.x >= platforms[i].x - 40
                            && hero.x + 90 <= platforms[i].x + platforms[i].img[0].Width + 110)
                        {
                            if (platforms[i].animationFlag == 1)
                            {
                                if (platformAnimation == 1)
                                    hero.verticalOffset = 1;
                                else
                                    hero.verticalOffset = -1;
                            }
                            else
                                hero.verticalOffset = 0;
                            previousLocation = 0;
                            if (hero.runningState == -1)
                                hero.idleState = 0;
                            hero.jumpingState = -1;
                            jumpCounter = 0;
                            flag = 1;
                            break;
                        }

                    }

                    if (flag == 0)
                    {

                        for (int i = 0; i < floors.Count; i++)
                        {
                            if (hero.y + 90 >= floors[i].y && hero.y + 90 <= floors[i].y + 100 && hero.x >= floors[i].x && hero.x + 90 <= floors[i].x + 360)
                            {
                                hero.verticalOffset = 0;
                                previousLocation = 0;
                                if(hero.runningState == -1)
                                    hero.idleState = 0;
                                hero.jumpingState = -1;
                                jumpCounter = 0;
                                break;
                            }
                            else
                            {
                                if (hero.y + 90 <= floors[i].y)
                                {

                                    hero.verticalOffset = 6;
                                    hero.jumpingState = 1;
                                    jumpCounter = 1;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            off = new Bitmap(ClientSize.Width, ClientSize.Height);
            createMyWorld();
            createUI();
            int floorX = 0, floorY = 500;
            for (int i = 0; i < 5; i++)
            {
                createFloors(ref floorX, ref floorY);
            }
            int boxX, boxY = floorY - 90;
            for (int i = 0; i < 2; i++)
            {
                boxX = 260;
                for (int z = 0; z < 2; z++)
                    createCrates(ref boxX, ref boxY);
                boxY += boxes[0].img[0].Height + 10;
            }
            floorX = 2700; floorY = 200;
            for (int i = 0; i < 5; i++)
            {
                createPlatform(floorX, 200, "platform-high.png", 3, 0);
                floorX += 380;
            }
            createPlatform(400, 400, "platform.png", 0, 0);
            createPlatform(700, 200, "platform-long.png", 1, 1);
            createPlatform(1500, 200, "platform-high.png", 3, 0);
            createLog(860, 260, 2);
            int gemX = 430;
            for (int i = 0; i < 3; i++)
            {
                createGems(gemX, 350);
                gemX += 50;
            }
            int trapX = 870;
            for (int i = 0; i < 3; i++)
            {
                createTrap(trapX, 290);
                trapX += 50;
            }
            createCherry(1300, 450);
            createItem(900, 5);
            createLaserBlock(1100, 10);
            createBulletBox(1500, 400);
            createEnemy(400, 300, 60, 0);
            createEnemy(3350, 60, 60, 0);
            createEnemy(850, 450, 60, 1);
            createEnemy(3150, 150, 60, 1);
            createEnemy(1050, 460, 240, 2);
            createLadder(1400, 180);
            createElevator(2600, 450);
            createLever(1850, 160);
            //decorations
            createLog();
            createHero();
        }
        void createUI()
        {
            ui.x = 40;
            ui.y = 600;
            ui.hpState = 0;
            ui.bulletState = 0;
            for (int i = 1; i <= 5; i++)
                ui.hp.Add(new Bitmap("hp-" + i + ".png"));
            for (int i = 1; i <= 5; i++)
                ui.bullet.Add(new Bitmap("bullets-" + i + ".png"));
            ui.profile = new Bitmap("profile.png");
        }
        void createBulletBox(int x,int y)
        {
            bulletBox laser = new bulletBox();
            laser.x = x;
            laser.y = y;
            laser.img = new Bitmap("laserblock.png");
            bulletBoxes.Add(laser);
        }
        void createCherry(int x, int y)
        {
            Cherry cherry = new Cherry();
            cherry.x = x;
            cherry.y = y;
            for (int i = 1; i <= 7; i++)
                cherry.idle.Add(new Bitmap("cherry-" + i + ".png"));
            for (int i = 1; i <= 4; i++)
                cherry.feedback.Add(new Bitmap("item-feedback-" + i + ".png"));
            cherries.Add(cherry);
        }
        void createGems(int x, int y)
        {
            Gem gem = new Gem();
            gem.x = x;
            gem.y = y;
            for (int i = 1; i <= 5; i++)
                gem.img.Add(new Bitmap("gem-" + i + ".png"));
            for (int i = 1; i <= 4; i++)
                gem.feedback.Add(new Bitmap("item-feedback-" + i + ".png"));
            gems.Add(gem);
        }
        void createTrap(int x, int y)
        {
            Trap trap = new Trap();
            trap.x = x;
            trap.y = y;
            trap.img = new Bitmap("spikes-top.png");
            traps.Add(trap);
        }
        void createLog()
        {
            int logX = 1930, logY = 140;
            for (int i = 0; i < 12; i++)
            {
                createDecortaiveLog(logX, logY, 0);
                logX += 40;
            }
            logX = 1930; logY = 460;
            for (int i = 0; i < 21; i++)
            {
                createDecortaiveLog(logX, logY, 0);
                logX += 40;
            }
            logX = 1930 + (40 * 12); logY = 140;
            for (int i = 0; i < 8; i++)
            {
                createDecortaiveLog(logX, logY, 1);
                logY += 40;
            }
            logX = 1930 + (40 * 21); logY = 140;
            for (int i = 0; i < 8; i++)
            {
                createDecortaiveLog(logX, logY, 1);
                logY += 40;
            }
            logX = 1930; logY = 140;
            for (int i = 0; i < 8; i++)
            {
                createDecortaiveLog(logX, logY, 1);
                logY += 40;
            }
        }
        void createCrates(ref int x, ref int y)
        {
            Boxes box = new Boxes();
            box.img.Add(new Bitmap("big-crate.png"));
            box.x = x; box.y = y;
            x += box.img[0].Width + 10;
            boxes.Add(box);
        }
        void createHero()
        {
            hero = new Hero();
            hero.x = 160;
            hero.y = 410;
            hero.hurt = new Bitmap("player-hurt.png");
            for (int i = 1; i <= 4; i++)
                hero.idle.Add(new Bitmap(@"player-idle-" + (i) + ".png"));
            for (int i = 1; i <= 6; i++)
                hero.runningRight.Add(new Bitmap(@"player-run-" + (i) + ".png"));
            for (int i = 1; i <= 6; i++)
                hero.runningLeft.Add(new Bitmap(@"player-run-left-" + (i) + ".png"));
            for (int i = 1; i <= 2; i++)
                hero.jumpingRight.Add(new Bitmap(@"player-jump-" + (i) + ".png"));
            for (int i = 1; i <= 2; i++)
                hero.jumpingLeft.Add(new Bitmap(@"player-jump-left-" + (i) + ".png"));
            for (int i = 1; i <= 6; i++)
                hero.startLaser.Add(new Bitmap(@"laser_S_" + i + ".png"));
            for (int i = 1; i <= 8; i++)
                hero.activeLaser.Add(new Bitmap(@"laser_A_" + i + ".png"));
            for (int i = 1; i <= 6; i++)
                hero.endLaser.Add(new Bitmap(@"laser_E_" + i + ".png"));
            for (int i = 1; i <= 6; i++)
                hero.startLaserLeft.Add(new Bitmap(@"laser_S_L_" + i + ".png"));
            for (int i = 1; i <= 8; i++)
                hero.activeLaserLeft.Add(new Bitmap(@"laser_A_L_" + i + ".png"));
            for (int i = 1; i <= 6; i++)
                hero.endLaserLeft.Add(new Bitmap(@"laser_E_L_" + i + ".png"));
            for (int i = 1; i <= 3; i++)
                hero.climbing.Add(new Bitmap(@"player-climb-" + (i) + ".png"));
        }
        void createFloors(ref int x, ref int y)
        {
            Floor floor = new Floor();
            floor.x = x;
            floor.y = y;
            floor.img.Add(new Bitmap("floor1.png"));
            floor.x += backgrounds[0].src.X;
            floor.y += backgrounds[0].src.Y;
            x += 290;
            floors.Add(floor);
        }
        void createPlatform(int x, int y, string img, int platformType, int animationFlag)
        {
            Floor floor = new Floor();
            floor.x = x;
            floor.y = y;
            floor.platform = platformType;
            floor.animationFlag = animationFlag;
            floor.img.Add(new Bitmap(img));
            floor.x += backgrounds[0].src.X;
            floor.y += backgrounds[0].src.Y;
            platforms.Add(floor);
        }
        void createLog(int x, int y, int type)
        {
            Floor floor = new Floor();
            floor.x = x;
            floor.y = y;
            floor.platform = type;
            floor.img.Add(new Bitmap("Log.png"));
            platforms.Add(floor);
        }
        void createItem(int x, int y)
        {
            Item item = new Item();
            item.x = x;
            item.y = y;
            for (int i = 1; i <= 4; i++)
                item.img.Add(new Bitmap("item-feedback-" + (i) + ".png"));
            item.state = 0;
            item.flag = 0;
            items.Add(item);
        }
        void createLaserBlock(int x, int y)
        {
            laserBox laser = new laserBox();
            laser.x = x;
            laser.y = y;
            laser.img = new Bitmap("laserblock.png");
            for (int i = 1; i <= 6; i++)
                laser.startLaser.Add(new Bitmap("laser_S_L_" + i + ".png"));
            for (int i = 1; i <= 8; i++)
                laser.activeLaser.Add(new Bitmap("laser_A_L_" + i + ".png"));
            for (int i = 1; i <= 6; i++)
                laser.endLaser.Add(new Bitmap("laser_E_L_" + i + ".png"));
            laserBlocks.Add(laser);
        }
        void createEnemy(int x, int y, int offset, int type)
        {
            if (type == 0)
            {
                Enemy enemy = new Enemy();
                enemy.type = type;
                enemy.x = x;
                enemy.y = y;
                enemy.rangeX = x - 30;
                enemy.rangeY = y - 30;
                enemy.rangeOffset = offset;
                for (int i = 1; i <= 4; i++)
                    enemy.img.Add(new Bitmap("eagle-" + i + ".png"));
                for (int i = 1; i <= 6; i++)
                    enemy.death.Add(new Bitmap("enemy-death-" + i + ".png"));
                enemies.Add(enemy);
            }
            else if (type == 1)
            {
                Frog frog = new Frog();
                frog.x = x; frog.y = y;
                frog.rangeY = y - 90;
                for (int i = 1; i <= 4; i++)
                    frog.idle.Add(new Bitmap("frog-idle-" + i + ".png"));
                for (int i = 1; i <= 2; i++)
                    frog.jump.Add(new Bitmap("frog-jump-" + i + ".png"));
                for (int i = 1; i <= 6; i++)
                    frog.death.Add(new Bitmap("enemy-death-" + i + ".png"));
                frogs.Add(frog);
            }
            else if (type == 2)
            {
                Enemy enemy = new Enemy();
                enemy.type = type;
                enemy.x = x;
                enemy.y = y;
                enemy.trueX = x;
                enemy.rangeX = x - 220;
                enemy.animationDirection = 0;
                enemy.moveOffset = -1;
                enemy.rangeOffset = offset;
                for (int i = 1; i <= 6; i++)
                    enemy.opussumLeft.Add(new Bitmap("opussum-left-" + i + ".png"));
                for (int i = 1; i <= 6; i++)
                    enemy.opussumRight.Add(new Bitmap("opussum-right-" + i + ".png"));
                for (int i = 1; i <= 6; i++)
                    enemy.death.Add(new Bitmap("enemy-death-" + i + ".png"));
                enemies.Add(enemy);
            }


        }
        void createLadder(int x, int y)
        {
            Ladder ladder = new Ladder();
            ladder.x = x;
            ladder.y = y;
            ladder.img.Add(new Bitmap("ladder.png"));
            ladders.Add(ladder);
        }
        void createElevator(int x, int y)
        {
            Elevator elevator = new Elevator();
            elevator.x = x;
            elevator.oldX = x;
            elevator.oldY = y;
            elevator.y = y;
            elevator.img = new Bitmap("elevator.png");
            elevators.Add(elevator);
        }
        void createLever(int x, int y)
        {
            Lever lever = new Lever();
            lever.x = x;
            lever.y = y;
            lever.state = 0;
            lever.img.Add(new Bitmap("crank-down.png"));
            lever.img.Add(new Bitmap("crank-up.png"));
            levers.Add(lever);
        }
        void createDecortaiveLog(int x, int y, int state)
        {
            decortaiveLogs log = new decortaiveLogs();
            log.x = x;
            log.y = y;
            log.state = state;
            if (state == 0)
                log.img = new Bitmap("decorative-log.png");
            else
                log.img = new Bitmap("decorative-log-up.png");
            logs.Add(log);
        }
        void createMyWorld()
        {
            Background pnn = new Background();
            pnn.src = new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height);
            pnn.dst = new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height);
            pnn.img = new Bitmap("environment.png");
            backgrounds.Add(pnn);
        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            drawDubb(e.Graphics);
        }
        void drawDubb(Graphics g)
        {
            Graphics g2 = Graphics.FromImage(off);
            drawScene(g2);
            g.DrawImage(off, 0, 0);
        }
        void drawScene(Graphics g)
        {
            g.Clear(Color.Black);
            for (int i = 0; i < backgrounds.Count; i++)
            {
                g.DrawImage(backgrounds[i].img, backgrounds[i].dst, backgrounds[i].src, GraphicsUnit.Pixel);
            }
            for (int i = 0; i < floors.Count; i++)
            {
                g.DrawImage(floors[i].img[0], floors[i].x, floors[i].y, 300, 500);
            }
            for (int i = 0; i < traps.Count; i++)
                g.DrawImage(traps[i].img, traps[i].x, traps[i].y, 40, 20);
            for (int i = 0; i < platforms.Count; i++)
            {
                if (platforms[i].platform == 0)
                {
                    g.DrawImage(platforms[i].img[0], platforms[i].x, platforms[i].y);
                }
                else if (platforms[i].platform == 1)
                {
                    g.DrawImage(platforms[i].img[0], platforms[i].x, platforms[i].y, 100, 40);
                }
                else if (platforms[i].platform == 2)
                {
                    int pushX = 0;
                    for (int z = 0; z < 5; z++)
                    {
                        g.DrawImage(platforms[i].img[0], platforms[i].x + pushX, platforms[i].y, 30, 35);
                        pushX += platforms[i].img[0].Width + 20;
                    }
                }
                else if (platforms[i].platform == 3)
                {
                    g.DrawImage(platforms[i].img[0], platforms[i].x, platforms[i].y, 400, 200);
                }

            }
            for (int i = 0; i < gems.Count; i++)
            {
                if (gems[i].taken == 0)
                    g.DrawImage(gems[i].img[gems[i].state], gems[i].x, gems[i].y, 32, 32);
                else if (gems[i].state != 4)
                    g.DrawImage(gems[i].feedback[gems[i].state], gems[i].x, gems[i].y, 32, 32);

            }
            for (int i = 0; i < boxes.Count; i++)
            {
                g.DrawImage(boxes[i].img[0], boxes[i].x, boxes[i].y, 50, 50);
            }
            for (int i = 0; i < items.Count; i++)
                if (items[i].state != 4)
                    g.DrawImage(items[i].img[items[i].state], items[i].x, items[i].y, 70, 70);
            for (int i = 0; i < laserBlocks.Count; i++)
            {
                g.DrawImage(laserBlocks[i].img, laserBlocks[i].x, laserBlocks[i].y, 90, 70);
                if (laserBlocks[i].laserFlag != -1)
                {
                    if (laserBlocks[i].laserFlag == 0)
                        g.DrawImage(laserBlocks[i].startLaser[laserBlocks[i].state], laserBlocks[i].x - 250, laserBlocks[i].y, 270, 70);
                    else if (laserBlocks[i].laserFlag == 1)
                        g.DrawImage(laserBlocks[i].activeLaser[laserBlocks[i].state], laserBlocks[i].x - 250, laserBlocks[i].y, 270, 70);
                    else if (laserBlocks[i].laserFlag == 2)
                        g.DrawImage(laserBlocks[i].endLaser[laserBlocks[i].state], laserBlocks[i].x - 250, laserBlocks[i].y, 270, 70);
                }
            }
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].type == 0)
                {
                    if (enemies[i].liveState == 1)
                        g.DrawImage(enemies[i].img[enemies[i].state], enemies[i].x, enemies[i].y, 70, 70);
                    else
                        g.DrawImage(enemies[i].death[enemies[i].state], enemies[i].x, enemies[i].y, 70, 70);
                }
                else
                {
                    if (enemies[i].type == 2)
                    {
                        if (enemies[i].liveState == 1)
                        {
                            if (enemies[i].animationDirection == 1)
                                g.DrawImage(enemies[i].opussumLeft[enemies[i].state], enemies[i].x, enemies[i].y, 70, 40);
                            else
                                g.DrawImage(enemies[i].opussumRight[enemies[i].state], enemies[i].x, enemies[i].y, 70, 40);
                        }
                        else
                            g.DrawImage(enemies[i].death[enemies[i].state], enemies[i].x, enemies[i].y, 70, 40);

                    }
                }
            }
            for (int i = 0; i < frogs.Count; i++)
            {
                if (frogs[i].liveState == 1)
                {
                    if (frogs[i].jumpFlag == 0)
                        g.DrawImage(frogs[i].idle[frogs[i].state], frogs[i].x, frogs[i].y, 70, 70);
                    if (frogs[i].jumpFlag == 1)
                        g.DrawImage(frogs[i].jump[frogs[i].state], frogs[i].x, frogs[i].y, 70, 70);
                }
                else
                    g.DrawImage(frogs[i].death[frogs[i].state], frogs[i].x, frogs[i].y, 70, 70);

            }
            for (int i = 0; i < ladders.Count; i++)
                g.DrawImage(ladders[i].img[0], ladders[i].x, ladders[i].y, 50, 320);
            for (int i = 0; i < logs.Count; i++)
            {
                if (logs[i].state == 0)
                    g.DrawImage(logs[i].img, logs[i].x, logs[i].y, 50, 30);
                else
                    g.DrawImage(logs[i].img, logs[i].x, logs[i].y, 30, 50);
            }
            for (int i = 0; i < elevators.Count; i++)
                g.DrawImage(elevators[i].img, elevators[i].x, elevators[i].y, 120, 120);
            for (int i = 0; i < levers.Count; i++)
                g.DrawImage(levers[i].img[levers[i].state], levers[i].x, levers[i].y, 60, 40);
            for (int i = 0; i < cherries.Count; i++)
            {
                if (cherries[i].taken == 0)
                    g.DrawImage(cherries[i].idle[cherries[i].state], cherries[i].x, cherries[i].y,50,50);
                else
                    g.DrawImage(cherries[i].feedback[cherries[i].state], cherries[i].x, cherries[i].y, 50, 50);
            }
            for(int i =0;i<bulletBoxes.Count;i++)
            {
                g.DrawImage(bulletBoxes[i].img, bulletBoxes[i].x, bulletBoxes[i].y,90,70);
                if (bulletBoxes[i].bullets.Count > 0)
                    for(int z = 0; z < bulletBoxes[i].bullets.Count;z++)
                        g.DrawImage(bulletBoxes[i].bullets[z].left, bulletBoxes[i].bullets[z].x, bulletBoxes[i].bullets[z].y);
            }
            //draw hero
            if (hero.boxes.Count != 0)
                g.DrawImage(hero.boxes[0].img[0], hero.boxes[0].x, hero.boxes[0].y, 80, 80);
            if (hero.hurtState != -1)
                g.DrawImage(hero.hurt, hero.x, hero.y, 70, 70);
            if (hero.idleState != -1 && hero.jumpingState == -1 && hero.hurtState == -1 && hero.climbingState == -1)
                g.DrawImage(hero.idle[hero.idleState], hero.x, hero.y, 90, 90);
            if (hero.runningState != -1 && hero.jumpingState == -1 && hero.hurtState == -1)
            {
                if (hero.directionFlag == 0)
                    g.DrawImage(hero.runningRight[hero.runningState], hero.x, hero.y, 90, 90);
                else
                    g.DrawImage(hero.runningLeft[hero.runningState], hero.x, hero.y, 90, 90);
            }
            if (hero.climbingState != -1 && hero.idleState == -1 && hero.jumpingState == -1)
            {
                g.DrawImage(hero.climbing[hero.climbingState], hero.x, hero.y, 90, 90);
            }
            if (hero.jumpingState != -1 && hero.hurtState == -1)
            {
                if (hero.directionFlag == 0)
                {
                    g.DrawImage(hero.jumpingRight[hero.jumpingState], hero.x, hero.y, 90, 90);
                }
                else
                {
                    g.DrawImage(hero.jumpingLeft[hero.jumpingState], hero.x, hero.y, 90, 90);
                }
            }
            if (hero.directionFlag == 0)
            {
                if (hero.laserFlag == 1)
                {
                    g.DrawImage(hero.startLaser[hero.laserState], hero.x + 90, hero.y + 30, 290, 70);

                }
                if (hero.laserFlag == 2)
                    g.DrawImage(hero.activeLaser[hero.laserState], hero.x + 90, hero.y + 30, 290, 70);
                if (hero.laserFlag == 3)
                    g.DrawImage(hero.endLaser[hero.laserState], hero.x + 90, hero.y + 30, 290, 70);
            }
            else
            {
                if (hero.laserFlag == 1)
                {
                    g.DrawImage(hero.startLaserLeft[hero.laserState], hero.x - 270, hero.y + 30, 290, 70);
                }
                if (hero.laserFlag == 2)
                    g.DrawImage(hero.activeLaserLeft[hero.laserState], hero.x - 270, hero.y + 30, 290, 70);
                if (hero.laserFlag == 3)
                    g.DrawImage(hero.endLaserLeft[hero.laserState], hero.x - 270, hero.y + 30, 290, 70);
            }
            if (hero.bullets.Count != 0)
                for (int i = 0; i < hero.bullets.Count; i++)
                    g.DrawImage(hero.bullets[i].img, hero.bullets[i].x, hero.bullets[i].y);
            g.DrawImage(ui.hp[ui.hpState / 5], ui.x, ui.y, 120, 20);
            g.DrawImage(ui.bullet[ui.bulletState / 10], ui.x, ui.y + 30, 120, 20);
            //g.DrawImage(ui.profile, ui.x - 20, ui.y - 15,20,50);
        }
    }
}
class UI
{
    public int x, y, hpState, bulletState;
    public Bitmap profile;
    public List<Bitmap> hp = new List<Bitmap>();
    public List<Bitmap> bullet = new List<Bitmap>();
}
class Cherry
{
    public int x, y, state= 0 , taken = 0;
    public List<Bitmap> idle = new List<Bitmap>();
    public List<Bitmap> feedback = new List<Bitmap>();
}
class Gem
{
    public int x, y, state = 0, taken = 0;
    public List<Bitmap> img = new List<Bitmap>();
    public List<Bitmap> feedback = new List<Bitmap>();
}
class Trap
{
    public int x, y;
    public Bitmap img;
}
class decortaiveLogs
{
    public int x, y, state;
    public Bitmap img;
}
class Lever
{
    public int x, y, state;
    public List<Bitmap> img = new List<Bitmap>();
}
class Elevator
{
    public int x, y, horizontalOffset = 0, verticalOffset = 0, moveState = 0, oldX, oldY;
    public Bitmap img;
}
class Ladder
{
    public int x, y;
    public List<Bitmap> img = new List<Bitmap>();
}
class Frog
{
    public int x, y, state, rangeX, rangeY, rangeOffset, animationDirection = 0, moveOffset = 1, liveState = 1, animationState = 0, jumpFlag;
    public List<Bitmap> idle = new List<Bitmap>();
    public List<Bitmap> jump = new List<Bitmap>();
    public List<Bitmap> death = new List<Bitmap>();
}
class Enemy
{
    public int x, y, state, rangeX, rangeY, rangeOffset, animationDirection = 0, moveOffset = 1, liveState = 1, type, trueX;
    public List<Bitmap> img = new List<Bitmap>();
    public List<Bitmap> death = new List<Bitmap>();
    public List<Bitmap> opussumLeft = new List<Bitmap>();
    public List<Bitmap> opussumRight = new List<Bitmap>();
}
class bulletBox
{
    public int x, y, state = 0, laserFlag = 0;
    public Bitmap img;
    public List<Bullet> bullets = new List<Bullet>();
}
class laserBox
{
    public int x, y, state = 0, laserFlag = 0;
    public Bitmap img;
    public List<Bitmap> startLaser = new List<Bitmap>();
    public List<Bitmap> activeLaser = new List<Bitmap>();
    public List<Bitmap> endLaser = new List<Bitmap>();
}
class Bullet
{
    public int x, y, directionFlag,interval;
    public Bitmap img = new Bitmap("bullet.png");
    public Bitmap left = new Bitmap("bullet1.png");
}
class Item
{
    public int x, y, state = 0, flag;
    public List<Bitmap> img = new List<Bitmap>();
}
class Floor
{
    public int x, y, flag = 0, platform = 0, animationFlag;
    public List<Bitmap> img = new List<Bitmap>();
}
class Background
{
    public Rectangle dst, src;
    public int offset;
    public Bitmap img;
}
class Boxes
{
    public int x, y, w = 10, h = 10, moveFlag = 0;
    public int verticalOffset = 0;
    public List<Bitmap> img = new List<Bitmap>();
}
class Hero
{
    public int x, y, idleState = 0, runningState = -1, jumpingState = -1, horizontalOffset = 0, verticalOffset = 0, directionFlag = 0, jumpFlag = 0;
    public int boxFlag = 0, laserFlag = 0, laserState = -1, hurtState = -1, doubleJump = 0, climbingState = -1, ladderFlag = -1, climb = 0;
    public Bitmap hurt;
    public List<Bullet> bullets = new List<Bullet>();
    public List<Bitmap> startLaser = new List<Bitmap>();
    public List<Bitmap> activeLaser = new List<Bitmap>();
    public List<Bitmap> endLaser = new List<Bitmap>();
    public List<Bitmap> startLaserLeft = new List<Bitmap>();
    public List<Bitmap> activeLaserLeft = new List<Bitmap>();
    public List<Bitmap> endLaserLeft = new List<Bitmap>();
    public List<Bitmap> runningRight = new List<Bitmap>();
    public List<Bitmap> runningLeft = new List<Bitmap>();
    public List<Bitmap> jumpingRight = new List<Bitmap>();
    public List<Bitmap> jumpingLeft = new List<Bitmap>();
    public List<Bitmap> climbing = new List<Bitmap>();
    public List<Boxes> boxes = new List<Boxes>();
    public List<Bitmap> idle = new List<Bitmap>();
}

