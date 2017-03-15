//
//  Screenshot.mm
//  ScreenShotPlugin
//
//  Created by Mayank Gupta on 06/01/15.
//  Copyright (c) 2015 Mayank Gupta. All rights reserved.
//


#import <Foundation/Foundation.h>

#import "Screenshot.h"

@interface Screenshot ()

@end

@implementation Screenshot
@synthesize library;
#pragma mark Unity bridge

+ (Screenshot *)pluginSharedInstance {
    static Screenshot *sharedInstance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        sharedInstance = [[Screenshot alloc] init];
    });
    return sharedInstance;
}

#pragma mark Cleanup
/*
- (void)dealloc {
    [super dealloc];
}*/
#pragma mark Ios Methods

-(bool)screenShotClick :(NSString *)albumName{
    NSArray *pathForDirectoriesInDomains = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory,     NSUserDomainMask, YES);
    NSString *documentsDirectory = [pathForDirectoriesInDomains objectAtIndex:0];
    NSString *getScreenshotPath = [documentsDirectory stringByAppendingPathComponent:@"Screenshot.png"];
    UIImage *theScreenshot = [UIImage imageWithContentsOfFile:getScreenshotPath];
    self.library = [[ALAssetsLibrary alloc] init];
    [self.library saveImage:theScreenshot toAlbum:albumName withCompletionBlock:^(NSError *error) {
        if (error!=nil) {
            NSLog(@"Big error: %@", [error description]);
        }
    }];
    if(theScreenshot == nil)
        return false;
    else
        return true;
}

- (void)image:(UIImage *)image didFinishSavingWithError:(NSError *)error contextInfo:(void *)contextInfo {
    if (error) {
        NSLog(@"--%@--" , error);
    }
}


@end

// Helper method used to convert NSStrings into C-style strings.
NSString *CreateStrScreenShot(const char* string) {
    if (string) {
        return [NSString stringWithUTF8String:string];
    } else {
        return [NSString stringWithUTF8String:""];
    }
}


// Unity can only talk directly to C code so use these method calls as wrappers
// into the actual plugin logic.
extern "C" { 
     bool _SaveScreenshotToGallery(const char *albumName){
        NSLog(@"----Entered Plugin");
        Screenshot *objScreenshot = [Screenshot pluginSharedInstance];
        NSLog(@"----Screenshot Saved To Gallery");
        return [objScreenshot screenShotClick:CreateStrScreenShot(albumName)];
    }
}
