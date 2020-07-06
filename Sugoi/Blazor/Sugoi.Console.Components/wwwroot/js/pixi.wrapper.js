
var offset = 0;
var buffer;
var screenArray;

window.createPixi = (container, width, height) => {
    //Create a Pixi Application
    let app = new PIXI.Application({width: width, height: height});
    //Add the canvas that Pixi automatically created for you to the HTML document
    container.appendChild(app.view);

    buffer = new Uint8Array(256 * 256 * 4);
    screenArray = new Uint8Array(256 * 256 * 4);

    var index = 0;
    for (var x = 0; x < 256 * 256; x++) {
        buffer[index + 0] = 0xFF; 
        buffer[index + 1] = x; 
        buffer[index + 2] = x; 
        buffer[index + 3] = 0xFF; 
        index += 4;
    }

    var texture = PIXI.Texture.fromBuffer(buffer, 256, 256);
    const bunny = PIXI.Sprite.from(texture);

    // center the sprite's anchor point
    bunny.anchor.set(0.5);

    // move the sprite to the center of the screen
    bunny.x = app.screen.width / 2;
    bunny.y = app.screen.height / 2;

    app.stage.addChild(bunny);

    // Listen for animate update
    app.ticker.add((delta) => {
        // just for fun, let's rotate mr rabbit a little
        // delta is 1 if running at 100% performance
        // creates frame-independent transformation

        DotNet.invokeMethodAsync('Sugoi.Console.Components', 'RenderAsync')
            .then(() => {

                window.BJSFDEJsFunctions.GetBinaryData("screen", screenArray);

                //console.log("*" + window.BJSFDEJsFunctions.GetBinaryData);
               console.log(screenArray);

                texture.update();
            })


        //console.log(screen);

        //DotNet.invokeMethodAsync('Sugoi.Console.Components', 'RenderAsync')
        //    .then(data => {
        //        //bunny.rotation = data;

        //        var index = 0;

        //        for (var x = 0; x < 256 * 256; x++) {
        //            buffer[index + 0] = x + offset;
        //            buffer[index + 1] = x + offset;
        //            buffer[index + 2] = x + offset;
        //            buffer[index + 3] = 0xFF;
        //            index += 4;
        //        }

        //        texture.update();

        //        offset++;
        //    });
    });
};
