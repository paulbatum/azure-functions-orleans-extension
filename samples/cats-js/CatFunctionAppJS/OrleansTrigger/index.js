class Cat {

    async eat (context, mystate) {        
        context.log('I have eaten ' + mystate + ' time(s).');
    }; 
}

const cat = new Cat();
module.exports = cat.eat;