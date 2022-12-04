class SimpleImage {
  static get toolbox() {
    return {
      title: 'Image',
      icon: '<svg width="17" height="15" viewBox="0 0 336 276" xmlns="http://www.w3.org/2000/svg"><path d="M291 150V79c0-19-15-34-34-34H79c-19 0-34 15-34 34v42l67-44 81 72 56-29 42 30zm0 52l-43-30-56 30-81-67-66 39v23c0 19 15 34 34 34h178c17 0 31-13 34-29zM79 0h178c44 0 79 35 79 79v118c0 44-35 79-79 79H79c-44 0-79-35-79-79V79C0 35 35 0 79 0z"/></svg>'
    };
  }

  constructor({data}){
    this.data = data;
    this.wrapper = undefined;
  }

   render(){
    this.wrapper = document.createElement('div');
    this.wrapper.classList.add('simple-image');

    if (this.data && this.data.url){
      this._createImage(this.data.url, this.data.caption);
      return this.wrapper;
    }
    const input = document.createElement('input');

    this.wrapper.classList.add('simple-image');
    this.wrapper.appendChild(input);

    input.placeholder = 'Paste an image URL...';
    input.value = this.data && this.data.url ? this.data.url : '';

    input.addEventListener('paste', (event) => {
      this._createImage(event.clipboardData.getData('text'));
    });

    return this.wrapper;
  }

  _createImage(url, captionText){
	const image = document.createElement('img');
	const caption = document.createElement('div');

	image.src = url;
	caption.contentEditable = true;
	caption.innerHTML = captionText || '';

	this.wrapper.innerHTML = '';
	this.wrapper.appendChild(image);
	this.wrapper.appendChild(caption);
  }


  validate(savedData){
    if (!savedData.url.trim()){
      return false;
    }

    return true;
  }

  save(blockContent){
    const image = blockContent.querySelector('img');
    const caption = blockContent.querySelector('[contenteditable]');

    return {
      url: image.src,
      caption: caption.innerHTML || ''
    }
  }
}

const editor = new EditorJS({
        	holder: 'editorjs',
        	tools: {
		        header: { 
		        	class: Header,
		        	inlineToolbar: true
		        },
		        list: { 
		        	class: List,
		        	inlineToolbar: true
		        },
                raw: RawTool,
            	checklist: {
                	class: Checklist,
		        	inlineToolbar: true
            	},
            	Color: {
			      class: window.ColorPlugin, 
			      config: {
			        colorCollections: [
			          "#FF1300",
			          "#EC7878",
			          "#9C27B0",
			          "#673AB7",
			          "#3F51B5",
			          "#0070FF",
			          "#03A9F4",
			          "#00BCD4",
			          "#4CAF50",
			          "#8BC34A",
			          "#CDDC39",
			          "#FFF"
			        ],
			        defaultColor: "#FF1300",
			        type: "text"
			      }
			    },
			    Marker: {
			      class: window.ColorPlugin, 
			      config: {
			         defaultColor: '#FFBF00',
			         type: 'marker', 
			      }       
			    },

		        image: {
			      class: SimpleImage
			    },

		        delimiter: { 
		        	class: Delimiter,
		        	inlineToolbar: ['link, bold']
		        },
		        //paragraph: Paragraph,
		        //embed: Embed,
		    }
    	});

function myFunction(){
    editor.save().then((output) => {
        console.log('Data: ', output);
    }).catch((error) => {
        console.log('Saving failed: ', error)
    });
}