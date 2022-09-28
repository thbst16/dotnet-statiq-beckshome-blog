Title: Functional Testing Attachment_Fu
Published: 7/21/2007
Tags:
    - Rails
    - Technology Guides
---
I was performing functional tests on my models that employed Attachment_Fu this morning and thought it would be worthwhile to share the code since it was a bit of a hassle pulling it together. Kudos to Mike Subelsky for his introduction to functional testing Attachment_Fu. It got me going in the right direction. What proved difficult once again was the multi-model controller. Once I got over that hump, I was on my way. As you can see from all the detail in the HTTP POST below, that was not an entirely easy task.

<pre data-enlighter-language="ruby">
class ProductsControllerTest < Test::Unit::TestCase
    ...
    def test_create_with_user    
        num_products = Product.count    
        imgdata = fixture_file_upload('/files/image.png', 'image/png')    
        audiodata = fixture_file_upload('/files/sound.mp3', 'audio/mpeg')    
        post :create, {
            :product => {
                :name => "Widget",
                :description => "A small tool-like item",
                :weight => "3",
                :price => "19.99",
                :language_id => "1";
            },
            :image => {:uploaded_data => imgdata},
            :audio => {:uploaded_data => audiodata} ,
            :html => { :multipart => true }
        },
        {:user_id => users(:valid_active_user).id}
        assert_response :redirect
        assert_redirected_to :action => 'show'
        assert_equal num_products + 1, Product.count
    end
    ...
end
</pre>