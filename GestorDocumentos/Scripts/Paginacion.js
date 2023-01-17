(function(){
    $.fn.paginate = function(options){
        var defaults = { itemforpage: 30, pagenumber: 5, selector: '#tabla' };
         options = $.extend(defaults, options);
         return this.each(function(){
            var object = $(this);
             var totalitems = $(`${defaults.selector } tbody tr`).length;
            var item_list = Math.ceil(totalitems / options.itemforpage);
             if (item_list > 1) {
                 //object.append('<li class="disabled"><a href="#!"><i class="material-icons">chevron_left</i></a></li>');
                 object.append('<li class="page-item disabled"><a class="page-link" href="#">Atras</a></li>');
               
            for (var i = 0; i < item_list; i++){
                //object.append('<li class="waves-effect"><a href="#!">'+ (i + 1) +'</a></li>');
                object.append(`<li class="page-item"><a class="page-link" href="#">${ (i + 1) }</a></li>`);
            }
            object.find('li').filter(':eq(1)').addClass('active');
            //object.append('<li id="pag_label" class="waves-effect"><span>1 de '+ item_list +'</span></li>');
            //object.append('<li><a href="#!"><i class="material-icons">chevron_right</i></a></li>');
            object.append('<li class="page-item"><a class="page-link" href="#">Siguiente</a></li>');    
            if (item_list == 1)
                object.find('li').filter(':last').addClass('disabled');
            $(`${defaults.selector} tbody tr`).filter(':gt('+ (options.itemforpage -1) +')').hide();
            object.find('li').filter(':gt('+ (options.pagenumber) +')').not(':last').hide();
            function thisitems(){
                var itemtohide = $(`${defaults.selector} tbody tr`).filter(':lt('+ ((i -1) *  options.itemforpage) +')');
                var itemtoshow = $(`${defaults.selector} tbody tr`).filter(':gt('+ (i *  options.itemforpage -1) +')');
                $.merge(itemtohide, itemtoshow);
                itemtohide.hide();
                $(`${defaults.selector} tbody tr`).not(itemtohide).show();
                object.find('li').removeClass('active');
                object.find('li').filter(':eq('+ i +')').addClass('active');
            }
            function thispage(index){
                var pagetohide = object.find('li').not(':first').not(':last').filter(':lt('+ index +')');
                var pagetoshow = object.find('li').not(':first').not(':last').filter(':gt('+ ((options.pagenumber -1) + index) +')');
                $.merge(pagetohide, pagetoshow);
                pagetohide.hide();
                object.find('li').not(pagetohide).show();
            }
            object.find('li a').not(':first').not(':last').click(function(e){
                e.preventDefault();
                i = parseInt($(this).text());
                object.find('li').filter(':last').removeClass('disabled');
                object.find('li').filter(':first').removeClass('disabled');
                if (i == 1){
                    object.find('li').filter(':first').addClass('disabled');
                    object.find('li').filter(':last').removeClass('disabled');
                 }
                 if (i == item_list){
                    object.find('li').filter(':last').addClass('disabled');
                    object.find('li').filter(':first').removeClass('disabled');
                  }
                thisitems();
            });
           
            var i = 1; 
            var pageindex = 0;
                 // Avanza hacia el final
            object.find('li a').filter(':last').on({
                click: function(event){
                    event.preventDefault();
                    object.find('li').filter(':first').removeClass('disabled');
                    i += 1;
                    if (object.find('li:eq('+ i +')').css('display') == 'none'){
                        pageindex += 1;
                        thispage(pageindex);
                    }
                    if (i >= item_list){
                        i = item_list;
                        $(this).parent().addClass('disabled');
                        object.find('li').filter(':first').removeClass('disabled');
                     }

                    thisitems();
                }
            });
                 // Avanza hacia el principio
            object.find('li a').filter(':first').on({
                click: function(event){
                    event.preventDefault();
                    object.find('li').filter(':last').removeClass('disabled');
                    i -= 1;
                    if (object.find('li:eq('+ i +')').css('display') == 'none'){
                        pageindex -= 1;
                        thispage(pageindex);
                    }
                    if (i <= 1){
                        i = 1;
                        $(this).parent().addClass('disabled');
                        object.find('li').filter(':last').removeClass('disabled');
                    }
                    thisitems();
                }
            });
            }
         });
    }
}(jQuery));

$('.pagination').paginate({ pagenumber: 5, itemforpage: 20, selector: '#tabla' }); 

function InputToUpper(obj) {
    if (obj.value != "") {
        obj.value = obj.value.toUpperCase();
    }
}
 
