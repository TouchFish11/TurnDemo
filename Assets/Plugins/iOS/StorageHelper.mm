#import <Foundation/Foundation.h>

extern "C" long long _GetFreeDiskSpace(const char* path) {
    NSString *nsPath = [NSString stringWithUTF8String:path];
    NSDictionary *attributes = [[NSFileManager defaultManager] attributesOfFileSystemForPath:nsPath error:nil];
    if (attributes) {
        NSNumber *freeSpace = [attributes objectForKey:NSFileSystemFreeSize];
        return [freeSpace longLongValue];
    }
    return -1;
}