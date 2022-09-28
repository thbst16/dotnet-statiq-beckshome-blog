Title: Attachment_Fu on Windows
Published: 7/19/2007
Tags:
    - Rails
    - Technology Guides
---
Continuing my Rails on Windows thread, I’m going to spend a bit of time on something that’s brought me both some substantial gains and some minor woes lately, running the Attachment_Fu plugin on Windows. I’ll start off with some general Attachment_Fu information and then get into some of the quirks, which are, as expected, mostly specific to the Windows environment.

![Attachment_Fu On Windows](https://s3.amazonaws.com/s3.beckshome.com/20070719-Attachment-Fu.gif)

First, for those not in the know, Attachment_Fu is a Rails plugin that allows you to store binary data (e.g. images, video, documents) and associate it with other models in your Rails application. Metadata (content type, size, height, width) about the attachment is stored in a separate model. Attachment_Fu’s sweet spot is handling images. It can handle automatic image conversion and thumbnailing using a number of popular image processors such as ImageScience, RMagick, or minmagick. Although not provided, you can imagine that Attachment_Fu might be extended to handle other types of binary processing utilities such as PDF converters or audio/video transcoding software. The other very cool thing about Attachment_Fu is that it provides support for pluggable persistence mechanisms. Out of the box, it allows for storage on the file system, as binary information in a database or on Amazon’s S3 storage service.

There is an abundance of information already written about Attachment_Fu so to avoid re-inventing the wheel, I’ll provide what I found to be the best sources of information to start.

* Mike Clark’s tutorial is the gold standard introduction to using Attachment_Fu. The code is simplistic but rock solid. It covers using both the file system and S3 for storage and will get you up and running on Attachment_Fu in no time.
* Some posts on the Attachment_Fu message board provide a solution to associating the attachment model with another model (i.e. making it an attachment to something). The posts provide both the controller and the view code for uploading the initial attachment and rendering it. Handling the attachment relationship in your MVC is going to be a fairly common requirement and most Attachment_Fu users will benefit from these posts.

For my part, I’m going to provide some controller source code for updating the attachment when you have a relationship with another model (an extension of the second item above) since this is one area that wasn’t covered well anywhere else and might save you some time in your travels. In the code below, my main model is the product and the image is the model where a photo and thumbnail are stored using Attachment_Fu.

<pre data-enlighter-language="ruby">
class ProductController < ApplicationController
    ...
    def update     @product = Product.find(params[:id])
        # Load up product categories for the view
        @all_categories = Category.find(:all, ::order=>"name_en")
        if @product.update_attributes(params[:product])
            if !params[:image][:uploaded_data].blank?
                # My product only has one image / thumbnail, I'll destroy 'each'
                # wait 3 # See quirk no.1 below
                @product.images.each {|img| img.destroy}
                @image = @product.images ||= Image.new
                @image = @product.images.build(params[:image])
                @image.save
            end
            flash[:notice] = 'Product was successfully updated.'
            redirect_to :action => 'show', :id => @product
            else
                render :action => 'edit'
            end
        end
    ...
end
</pre>
The links above, in combination with my snippet, should get you through creating an attachment and handling CRUD for an attachment and its parent model from a single view. Now comes the Windows quirkiness. Not knowing to expect these Attachment_Fu quirks and then having to root out the cause of the behavior took up a lot of time. It turns out that most of I found that most of the quirks are documented in some way, shape, or form. I’ve pulled together a list of the quirks as well as some best practice workarounds.

* When running Attachment_Fu on Windows, the most commonly accounted problem is the “Size Is Not Included In List” validation error. It appears that no amount of fixing in the Ruby code is going to help here since it appears to be a Windows file system issue. The workaround is really simple, just add a wait x statement before your attachment processing and things will be golden. The x (which denotes seconds) time will vary based upon the size of the attachments you are processing. Bigger attachments require more of a wait. Also, be sure to comment this code out in production since this is a Windows only issue.
    
    <span style="color:red"><i>7/19/2007 Update – Rick suggested using RUBY_PLATFORM to determine if the wait should be invoked. I tested this and it worked as suggested</i></span>

* When you invoke the destroy method on your attachment using Attachment_Fu on Windows, your models reference to the attachment will be deleted but the physical attachments themselves will not be deleted if you have persisted them to the file system. If you look at the Attachment_Fu source code or your log files, you’ll see that Attachment_Fu assumes that you are using a UNIX-based system and executes UNIX commands like rm to remove these files. These commands will obviously not work in a Windows environment, leaving you with a bunch of zombie files. This should not be a problem if you use a database or S3 persistence mechanism since these mechanisms are independent of the operating system.
    
    <span style="color:red"><i>7/19/2007 Update – Rick corrected me. He is indeed calling the OS safe FileUtils.rm in the file system backend. It still isn’t working though – at least on my machine.</i></span>

* My last Windows specific quirk is actually an Internet Explorer issue. If your attachments are images, you may have problems with uploading JPEG’s using the default Attachment_Fu plugin. From what I’ve been able to determine, if you upload a JPEG from IE with a file extension of .JPEG, IE will set the MIME type to image/pjpeg for a progressive JPEG. However, if the image extension is simply .jpg, IE will set the MIME type to image/jpg. This MIME type, however, is not included in the default list of content types accepted by Attachment_Fu. My suggestion is to add this type to the list in the source code until Rick can get around to modifying the source.
    
    <span style="color:red"><i>7/19/2007 Update – The MIME type was added to source. For reference, Rick suggested that this could have been done without changing the source simply by adding
Technoweenie::AttachmentFu.content_types << ‘image/jpg</i></span>

The last quirk for my post should be meaningful to all of those using Capistrano, the Rails migration utility. Capistrano manages versions of the application for rollforward / rollback by creating symlinks to previous versions of an application and deploying the most recent version of your entire application tree from your version control system (e.g. Subversion). However, since it’s very unlikely that you are storing all of the attachments for your application under version control, the attachments will be unlinked and no longer available when you migrate a new version of your application to production. To get around this issue, the solution proposed here creates a separate physical directory for the attachments outside of your application’s directory and then updates a symlink from your application’s attachment directory to the separate physical directory every time you migrate.